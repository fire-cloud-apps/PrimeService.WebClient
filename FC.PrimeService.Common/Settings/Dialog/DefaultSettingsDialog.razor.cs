using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class DefaultSettingsDialog
{
 
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public DefaultSettings _defaultSettings { get; set; } 
    private bool _processing = false;
    MudForm form;
    private DefaultSettings _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    #endregion

    #region Component Initialization
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        
        _inputMode = await GetDefault();
        if (_inputMode == null)
        {
            //Occurs only during very first time.
            _inputMode = new DefaultSettings()
            {
                Deadline = 5,
                Warranty = 90
            };
        }

        _loading = false;
        StateHasChanged();
    }
    #endregion
    
    #region Get Profile Details
    private async Task<DefaultSettings> GetDefault()
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketDefaultApi.GetDefault}";
        var responseModel = await _httpService.GET<DefaultSettings>(url);
        return responseModel;
    }

    #endregion
    
    #region Submit Button with Animation

    private async Task Submit()
    {
        await form.Validate();
        if (form.IsValid)
        {
            var isSuccess = await SubmitAction();
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "User Profile Updated!");
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Utilities.ConsoleMessage(_outputJson);
        }
    }

    async Task<bool> SubmitAction()
    {
        _processing = true;
        string url = string.Empty;
        DefaultSettings responseModel = null;
        bool result = false;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketDefaultApi.AddOrUpdate}";
        responseModel = await _httpService.PUT<DefaultSettings>(url, _inputMode);
        result = (responseModel != null);
        
        Utilities.ConsoleMessage($"Executed API URL : {url}");
        Utilities.ConsoleMessage($"'DefaultSettings' JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    #endregion
}