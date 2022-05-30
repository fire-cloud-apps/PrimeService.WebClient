using System.Text.Json;
using FC.PrimeService.Tickets.ActivityTask.Dialog;
using FC.PrimeService.Tickets.PaymentDetail.Dialog;
using MudBlazor;
using PrimeService.Model.Tickets;
using Model = PrimeService.Model.Tickets;

namespace FC.PrimeService.Tickets.Ticket.Component;

public partial class TicketComponent
{
    
    #region Submit Button with Animation
    async Task ProcessSomething()
    {
        _processing = true;
        await Task.Delay(2000);
        _processing = false;
    }
    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // //Todo some animation.
            await ProcessSomething();

            //Do server actions.
            _outputJson = JsonSerializer.Serialize<TicketService?>(_inputMode);

            //Success Message
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            //Snackbar.Configuration.VisibleStateDuration  = 2000;
            //Can also be done as global configuration. Ref:
            //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
            Snackbar.Add("Submited!", Severity.Success);
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine((string?)_outputJson);
        }
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
    
    private async Task InvokeDialog(string parameter, string title, Model.PaymentDetails model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        IDialogReference dialog;

        dialog = DialogService.Show<PaymentDetailsDialog>(title, parameters, _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    private async Task InvokeTaskDialog(string parameter, string title, Model.ActivityTasks model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
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
        await InvokeDialog("_PaymentDetails", "Add Payments", null);
    }

    private async Task AddTask()
    {
        await InvokeTaskDialog("_TaskDetails", "Add Tasks", null);
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
    private string _key, _value;

    private List<AdditionalKeyValue> keyList = new List<AdditionalKeyValue>()
    {
        new AdditionalKeyValue()
    };
    
    private void AddKeyValue()
    {
        //TODO:Update a portion of field
        //https://stackoverflow.com/questions/33227497/how-to-save-a-portion-of-a-large-document-mongodb-c-sharp-driver
        keyList.Add(new AdditionalKeyValue());
        StateHasChanged();
        
    }
    private void RemoveKeyValue(AdditionalKeyValue keypair)
    {
        keyList.Remove(keypair);
        StateHasChanged();
    }
    
    #endregion
    
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