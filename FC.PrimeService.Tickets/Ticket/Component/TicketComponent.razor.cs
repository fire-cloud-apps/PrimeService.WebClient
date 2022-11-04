using System.Text.Json;
using FC.PrimeService.Payments.Payment.Dialogs;
using FC.PrimeService.Shopping.Client.Dialog;
using FC.PrimeService.Tickets.ActivityTask.Dialog;
using Humanizer;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Shopping;
using PrimeService.Model.Tickets;
using PrimeService.Model.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Tickets;
using PayModel = PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Tickets.Ticket.Component;

public partial class TicketComponent
{
    
    #region Submit, Delete, Cancel Button with Animation

    private async Task PayLater()
    {
        await Submit();
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var isSuccess = await SubmitAction(_userAction);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Ticket Details Saved!");
                //windowReload
                await JSRuntime.InvokeAsync<bool>("windowReload",null);
            }
        }
        else
        {
            _outputJson = "Validation Error occured.";
        }
        Utilities.ConsoleMessage(_outputJson);
    }

    private Dictionary<string, IList<CustomField>> _customProperties = new Dictionary<string, IList<CustomField>>();
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        _loading = true;
        string url = string.Empty;
        Model.TicketService responseModel = null;
        bool result = false;
        try
        {
            switch (action)
            {
                case UserAction.ADD:
                    if (string.IsNullOrEmpty(_inputMode.UserLastComments))
                    {
                        _inputMode.UserLastComments = $"Service '{_inputMode.ServiceType.Title}', has been initiated for the client '{_inputMode.Client.Name}'. Paid Amount {_inputMode.PaidAmount} Balance Amount '{_inputMode.BalanceAmount}' as of Date '{DateTime.Now.Humanize()}'";
                    }
                    _inputMode.TicketTypeDetails = _customProperties;
                    _inputMode.Activities = new List<ActivityLog>()
                    {
                        new()
                        {
                            ActivityDate = DateTime.Now,
                            AssignedFrom = _inputMode.AssignedTo,
                            AssignedTo = _inputMode.AssignedTo,
                            ByWho = _loginUser,
                            FromStatus = _inputMode.TicketStatus,
                            ToStatus = _inputMode.TicketStatus,
                            UserComments = _inputMode.UserLastComments,
                            Log = $"Ticket Created {_inputMode.TicketNo}, Assigned to {_inputMode.AssignedTo.Name}"
                        }
                    };
                    _inputMode.TaskList = new List<ActivityTasks>()
                    {
                        new()
                        {
                            AssignedTo = _inputMode.AssignedTo,
                            CompletedDate = _inputMode.TargetDate,
                            IsCompleted = false,
                            TargetDate = _inputMode.TargetDate,
                            TicketNo = _inputMode.TicketNo,
                            Notes = "A New task created",
                            Title = _inputMode.ServiceType.Type.ToString().Humanize(LetterCasing.Title)
                        }
                    };
                    url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.Create}";
                    responseModel = await _httpService.POST<Model.TicketService>(url, _inputMode);
                    result = (responseModel != null);
                    break;
                case UserAction.EDIT:
                    url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.Update}";
                    responseModel = await _httpService.PUT<Model.TicketService>(url, _inputMode);
                    result = (responseModel != null);
                    break;
                case UserAction.DELETE:
                    url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.Delete}";
                    url = string.Format(url, _inputMode.Id);
                    result = await _httpService.DELETE<bool>(url);
                    break;
                default:
                    break;
            }
            Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        }
        finally
        {
            _processing = false;
            _loading = false;
        }
        return result;
    }
    
    async Task Delete()
    {
        var canDelete = await Utilities.DeleteConfirm(DialogService);
        if (canDelete)
        {
            await SubmitAction(UserAction.DELETE);
            Utilities.SnackMessage(Snackbar, "Client Deleted!", Severity.Warning);
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    #endregion

    #region Amount Calculation

    private bool _paymentDisable = false;
    //private double _payedAmount = 0.0d;
    private void CalculateAmount()
    {
        double income = 0;
        double refund = 0;
        //All payment calculation
        foreach (var payments in _inputMode.Payments)
        {
            income += payments.IncomeAmount;
            refund += payments.ExpenseAmount;
        }
        //Refund calculation is also calculated.
        double payedAmount = income - refund;
        
        //Overall calculation
        Utilities.ConsoleMessage($"0. Overall: Cost {_inputMode.ServiceType.Cost} Price {_inputMode.ServiceType.Price} Payed Amount {payedAmount}");
        double totalAmount = _inputMode.ServiceType.Cost +
                             _inputMode.ServiceType.Price;
        double balance = totalAmount - (_inputMode.AdvanceAmount + payedAmount);
       
        _inputMode.TotalAmount = totalAmount;
        _inputMode.BalanceAmount = balance;
        _inputMode.PaidAmount = payedAmount;
        Utilities.ConsoleMessage($"1. Income {income} Refund {refund} Balance {balance} Total {totalAmount}");
        _paymentDisable = payedAmount == totalAmount;
    }

    #endregion
    
    #region Fake Data

    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region Add Payment Dialog
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Medium,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private async Task InvokeDialog(string parameter, string title, global::PrimeService.Model.Settings.Payments.Payments model)
    {
        var parameters = new DialogParameters
        {
            [parameter] = model,
            ["Title"] = title,
            ["UserAction"] = UserAction.NA
        };
        IDialogReference dialog;
        dialog = DialogService.Show<PaymentDialog>(title, parameters, _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            PayModel.Payments payments = new PayModel.Payments();
            payments = result.Data as PayModel.Payments;
            payments.Id = Guid.NewGuid().ToString();
            Utilities.ConsoleMessage($"Payment Id {payments.Id}");
            //Add the items in the list
            _inputMode.Payments.Add(payments);
            CalculateAmount();
        }
    }
    private async Task InvokeTaskDialog(string parameter, string title, Model.ActivityTasks model)
    {
        var parameters = new DialogParameters
        {
            [parameter] = model,
            ["TaskList"] = _inputMode.TaskList,
            ["Title"] = title,
            ["UserAction"] = UserAction.ADD
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        IDialogReference dialog;

        _dialogOptions.MaxWidth = MaxWidth.Small; 
        dialog = DialogService.Show<ActivityTaskDialog>(title, parameters, _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    #endregion

    private async Task InvokeAddPaymentDialog()
    {
        //PrimeService.Model.Settings.Payments
        PayModel.Payments payments = new PayModel.Payments()
        {
            Client = _inputMode.Client,
            PaymentMethod = await Utilities.GetDefault_PaymentMethods( _appSettings, _httpService) ,
            PaymentTag = await Utilities.GetDefault_PaymentAccount(_appSettings, _httpService),
            Who = _loginUser,
            BillNo = _inputMode.TicketNo,
            IncomeAmount = _inputMode.BalanceAmount,
            PaymentCategory = PayModel.PaymentCategory.Income,
            TransactionDate = DateTime.Now,
            PaymentStatus = PaymentStatus.PartiallyPaid
            //Reason = 
        };
        
        await InvokeDialog("Payments", "Add Payments", payments);
    }
    
    private async Task InvokeRefundPaymentDialog()
    {
        //PrimeService.Model.Settings.Payments
        PayModel.Payments payments = new PayModel.Payments()
        {
            Client = _inputMode.Client,
            PaymentMethod = await Utilities.GetDefault_PaymentMethods( _appSettings, _httpService) ,
            PaymentTag = await Utilities.GetDefault_PaymentAccount(_appSettings, _httpService),
            Who = _loginUser,
            BillNo = _inputMode.TicketNo,
            ExpenseAmount = _inputMode.TotalAmount,
            PaymentStatus = PaymentStatus.Refund,
            PaymentCategory = PayModel.PaymentCategory.Refund,
            TransactionDate = DateTime.Now,
            //Reason = 
        };
        
        await InvokeDialog("Payments", "Refund Payments", payments);
    }

    private async Task AddTask()
    {
        await InvokeTaskDialog("TaskDetails", "Add Tasks", new ActivityTasks()
        {
            TicketNo = _inputMode.TicketNo,
        });
    }

    private async Task ExpandTicket()
    {
        var url = $"/TicketFullView?Id={_inputMode.Id}";
        //Navigate and open in new tab.
        await JSRuntime.InvokeAsync<object>("open",
            new object[2] { url, "_blank" });
    }

    #region Additional Details
    private int _additionItemIndex = 0;
    

   
    
    private void AddKeyValue()
    {
        
        //TODO:Update a portion of field
        //https://stackoverflow.com/questions/33227497/how-to-save-a-portion-of-a-large-document-mongodb-c-sharp-driver
        _inputMode.AdditionalDetails.Add(new CustomField());
        StateHasChanged();
        
    }
    private void RemoveKeyValue(CustomField keypair)
    {
        _inputMode.AdditionalDetails.Remove(keypair);
        StateHasChanged();
    }
    
    #endregion
    
    private async Task AddClientDialog()
    {
        var parameters = new DialogParameters
            { ["Client"] = null }; 
        IDialogReference dialog;
        
        dialog = DialogService.Show<ClientDialog>(
            string.Empty, 
            parameters,
            _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    // private async Task ServiceType_Changed(string arg)
    // {
    //     Utilities.ConsoleMessage($"Service Type {arg}");
    // }
}

public class AdditionalKeyValue
{
  public  string Key
  {
      get;
      set;
  } 
  public  string Value
  {
      get;
      set;
  } 

}