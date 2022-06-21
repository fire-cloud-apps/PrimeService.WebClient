using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Common;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class SequenceDialog
{
    #region Global Variables
    
    private Modules _selectedModule = Modules.Sales;
    private Modules SelectedModule
    {
        get => _selectedModule;
        set
        {
            _selectedModule = value;
            Utilities.ConsoleMessage($"Selected Modules -> {_selectedModule}");
            GetDefaultSettings();
            //API Call here to get the Sequence settings for the current module
        }
    } 

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public SerialNumber _SerialNumber { get; set; } //This comes from 'Dialog' invoker.
    private bool _processing = false;
    MudForm form;
    private SerialNumber _inputMode;
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
        
        if (_SerialNumber == null)
        {
            _inputMode = new SerialNumber();
            //Dialog box opened in "Add" mode
            _selectedModule = Modules.Sales;
            GetDefaultSettings();
            _title = "Serial Number";
            _userAction = UserAction.ADD;
            _readOnlyOrder = false;
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _SerialNumber;
            _title = "Serial Number";
            _userAction = UserAction.EDIT;
            _readOnlyOrder = true;
        }

        Utilities.ConsoleMessage($"Mode : {_userAction}");
    }
    
    /// <summary>
    /// Get Default Settings
    /// </summary>
    async void  GetDefaultSettings()
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SequenceApi.GetDetails}";
        url = string.Format(url, _selectedModule);
        var result = await _httpService.GET<SerialNumber>(url);
        Utilities.ConsoleMessage(result.ToJson());
        if (result == null)
        {
            _inputMode = new SerialNumber()
            {
                BatchNo = DateTime.Now.Year,
                Separator = ".",
                Prefix = "FC",
                Suffix = "#",
                TableName = "TicketService"
            };
            Utilities.ConsoleMessage("Module Request - No Data");
        }
        else
        {
            _inputMode = result;
            Utilities.ConsoleMessage("Module Request - Success");
        }

        StateHasChanged();
    }
    
    #endregion
    
    #region Submit, Delete, Cancel Button with Animation
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        SerialNumber responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SequenceApi.Generate}";
                responseModel = await _httpService.POST<SerialNumber>(url, _inputMode);
                result = (responseModel != null);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Status JSON : {_inputMode.ToJson()}");
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
            Utilities.SnackMessage(Snackbar, "SerialNumber Saved!");
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Utilities.ConsoleMessage(_outputJson);
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

public enum Modules
{
    TicketService,
    Sales
}


