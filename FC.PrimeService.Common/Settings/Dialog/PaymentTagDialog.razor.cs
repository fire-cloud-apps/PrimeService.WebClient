using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class PaymentTagDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public PaymentTags _PaymentTags { get; set; } 
    private bool _processing = false;
    MudForm form;
    private PaymentTags _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    
    #region Dialog Parameters
    [Parameter] public PaymentTags PaymentTags { get; set; } 
    [Parameter] public string Title { get; set; }
    [Parameter] public UserAction UserAction { get; set; }
    #endregion
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        Utilities.ConsoleMessage($"Payment Tags - User Action : {UserAction}");
        if (UserAction == UserAction.ADD)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new PaymentTags();//Initializes an empty object.
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = PaymentTags;
        }

        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Submit, Delete, Cancel Button with Animation

    private string _eventMessage = "PaymentTags Saved!";
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var isSuccess = await SubmitAction(UserAction);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, _eventMessage);
                MudDialog.Close(DialogResult.Ok(true));
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
        PaymentTags responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentTagsApi.Create}";
                responseModel = await _httpService.POST<PaymentTags>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                if (_inputMode.IsDefault)
                {
                    result = await SetDefaultAccount();
                    _eventMessage = $"Payment Tag : '{_inputMode.Title}' set as Default Account.";
                }
                else
                {
                    url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentTagsApi.Update}";
                    responseModel = await _httpService.PUT<PaymentTags>(url, _inputMode);
                    result = (responseModel != null);
                }
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentTagsApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"PaymentTags JSON : {_inputMode.ToJson()}");
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
            Utilities.SnackMessage(Snackbar, "PaymentTags Deleted!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    #endregion

    private async Task<bool> SetDefaultAccount()
    {
        string url = string.Empty;
        PaymentTags responseModel = null;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentTagsApi.SetDefault}";
        responseModel = await _httpService.PUT<PaymentTags>(url, _inputMode);
        var result = (responseModel != null);
        return result;
    }
}