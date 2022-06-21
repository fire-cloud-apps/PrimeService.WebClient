using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class StatusDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public Status _status { get; set; } //This comes from 'Dialog' invoker.
    private bool _processing = false;
    MudForm form;
    private Status _inputMode;
    string _outputJson;
    bool success;
    bool _readOnlyOrder;
    string[] errors = { };
    private bool _isReadOnly = false;
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    private UserAction _userAction;
    
    #endregion

    #region Component Initialization
    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        
        if (_status == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new Status()
            {
                Name = string.Empty,
                ColorCode = "#c9cccf",
            };//Initializes an empty object.
            _title = "Status";
            _userAction = UserAction.ADD;
            _readOnlyOrder = false;
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _status;
            _title = "Status";
            _userAction = UserAction.EDIT;
            _readOnlyOrder = true;
        }

        Console.WriteLine($"Mode : {_userAction}");
    }
    #endregion
    
    #region Submit, Delete, Cancel Button with Animation
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
            Utilities.SnackMessage(Snackbar, "Status Deleted!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        
        
        StateHasChanged();
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        Status responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketStatusApi.Create}";
                responseModel = await _httpService.POST<Status>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketStatusApi.Update}";
                responseModel = await _httpService.PUT<Status>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketStatusApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Console.WriteLine($"Executed API URL : {url}, Method {action}");
        Console.WriteLine($"Status JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // //Todo some animation.
            await SubmitAction(_userAction);
            //Do server actions.
            _outputJson = JsonSerializer.Serialize(_inputMode);
            Utilities.SnackMessage(Snackbar, "Status Saved!");
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine(_outputJson);
        }
    }

    #endregion

    #region Fake Data
    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }
    #endregion
}