using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Shop = PrimeService.Model.Shopping;
using Model = PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Payments.Payment.Dialogs;

public partial class PaymentDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    
    #region Dialog Parameters
    [Parameter] public Model.Payments Payments { get; set; } 
    [Parameter] public string Title { get; set; }
    [Parameter] public UserAction UserAction { get; set; }
    #endregion
    
    private bool _processing = false;
    MudForm form;
    private Model.Payments _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private string _detectedHeight = "450";
    private string _dialogBehaviour = "max-height:{0}px; overflow-y: scroll; overflow-x: hidden;";
    private bool _isReadOnly = false;
    private MudButton submitButton;
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager, 
            _localStore, _configuration, Snackbar);
        _inputMode = Payments;
    }
    #endregion
    
    
    #region Client Search - Autocomplete

    private IEnumerable<Shop.Client> _clients = new List<Shop.Client>();

    private async Task<IEnumerable<Shop.Client>> Client_SearchAsync(string value)
    {
        var responseData = await Utilities.GetClients(_appSettings, _httpService, value, "Name");
        _clients = responseData.Items;
        Console.WriteLine($"Find Client : '{value}'" );
        return _clients;
    }

    #endregion
    
    #region PaymentTags Search - Autocomplete

    private IEnumerable<Model.PaymentTags> _paymentTags = new List<Model.PaymentTags>();
    private async Task<IEnumerable<Model.PaymentTags>> PaymentTag_SearchAsync(string value)
    {
        var responseData = await Utilities.GetPaymentTags(_appSettings, _httpService, value);
        _paymentTags = responseData.Items;
        Console.WriteLine($"Find Payment Tags : '{value}'" );
        return _paymentTags;
    }

    #endregion

    #region PaymentMethods Search - Autocomplete

    IEnumerable<Model.PaymentMethods> _paymentMethods = new List<Model.PaymentMethods>();

    async Task<IEnumerable<Model.PaymentMethods>> PaymentMethod_SearchAsync(string value)
    {
        var responseData = await Utilities.GetPaymentMethods(_appSettings, _httpService, value);
        _paymentMethods = responseData.Items;
        Console.WriteLine($"Find Payment Tags : '{value}'" );
        return _paymentMethods;
    }

    #endregion
    
    #region Submit, Delete, Cancel Button with Animation

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            if (await SubmitAction(UserAction))
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Payment Details Saved!");
                MudDialog.Close(DialogResult.Ok(_inputMode));
            }
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Utilities.ConsoleMessage(_outputJson);
        }
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        Model.Payments responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
                _inputMode.TransactionDate = DateTime.Now;
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentApi.Create}";
                responseModel = await _httpService.POST<Model.Payments>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentApi.Update}";
                responseModel = await _httpService.PUT<Model.Payments>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            case UserAction.NA:
                result = true;//Action should taken the receiver. Eg. in TicketService
                if (_inputMode.PaymentCategory == Model.PaymentCategory.Income)
                {
                    _inputMode.Reason = $"Client {_inputMode.Client.Name} Paid the Amount of {_inputMode.IncomeAmount}.";
                }
                else
                {
                    _inputMode.Reason = $"Amount of {_inputMode.IncomeAmount} has been refunded to the client {_inputMode.Client.Name}.";
                }
                
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Payments JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }
    private void Cancel()
    {
        MudDialog.Cancel();
    }

    async Task Delete()
    {
        var canDelete = await Utilities.DeleteConfirm(DialogService);
        if (canDelete)
        {
            await SubmitAction(UserAction.DELETE);
            Utilities.SnackMessage(Snackbar, "Payments Deleted!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    #endregion
}