using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class UserProfileDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    [Parameter] public User _loginUser { get; set; } 
    private bool _processing = false;
    MudForm form;
    private Employee _inputMode;
    string _outputJson;
    bool success;
    private bool _isReadOnly = false;

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
        
        if (_loginUser != null)
        {
            Utilities.ConsoleMessage($"User Profile - User Info: {_loginUser.Id} - {_loginUser.Username}");
            _inputMode = await GetProfile(_loginUser.Username);
            Utilities.ConsoleMessage($"User Data {JsonSerializer.Serialize<Employee>(_inputMode)}");
            
        }
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Get Profile Details
    private async Task<Employee> GetProfile(string email)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.GetByEmail}";
        url = string.Format(url, email);
        var responseModel = await _httpService.GET<Employee>(url);
        return responseModel;
    }

    #endregion
    
    #region Submit Button with Animation

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            //Todo some animation.
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
        Employee responseModel = null;
        bool result = false;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.Update}";
        responseModel = await _httpService.PUT<Employee>(url, _inputMode);
        result = (responseModel != null);
        
        Utilities.ConsoleMessage($"Executed API URL : {url}");
        Utilities.ConsoleMessage($"Employee JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    #endregion

    #region Password Toggle
    bool PasswordVisibility;
    InputType PasswordInput = InputType.Password;
    string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    void TogglePasswordVisibility()
    {
        if (PasswordVisibility)
        {
            PasswordVisibility = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            PasswordVisibility = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }

    #endregion

}