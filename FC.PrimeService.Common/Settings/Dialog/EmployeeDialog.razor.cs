using System.Text.Json;
using FireCloud.WebClient.PrimeService.Service.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class EmployeeDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    
    #region Dialog Parameters
    [Parameter] public Employee Employee { get; set; } 
    [Parameter] public UserAction UserAction { get; set; } 
    [Parameter] public string Title { get; set; } 
    [Parameter] public User LoginUser { get; set; } 
    
    #endregion
    
    private bool _processing = false;
    MudForm form;
    private Employee _inputMode;
    string _outputJson;
    bool success;
    private string _detectedHeight = "450";
    private string _dialogBehaviour = "max-height:{0}px; overflow-y: scroll; overflow-x: hidden;";
    private bool _isReadOnly = false;

    private IEnumerable<WorkLocation> _workLocations = new List<WorkLocation>(); 
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        Utilities.ConsoleMessage($"Employee - User Action : {UserAction}");
        if (UserAction == UserAction.ADD)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new Employee()
            {
                User = new User(),
                WorkLocation = new WorkLocation()
            };
            Utilities.ConsoleMessage($"Input Mode Assigned.");
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = Employee;
        }
        Utilities.ConsoleMessage($"Load Completed.");
        StateHasChanged();
    }
    #endregion
    
    #region WorkLocation Search - Autocomplete

    private async Task<IEnumerable<WorkLocation>> WorkLocation_SearchAsync(string value)
    {
        var responseData = await GetDataByBatch(value);
        _workLocations = responseData.Items;
        return _workLocations;
    }

    #region WorkLocation - AutoComplete Ajax call
    private async Task<ResponseData<WorkLocation>> GetDataByBatch(string searchValue)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.WorkLocationApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = "Name",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<WorkLocation>>(url, pageMetaData);
        return responseModel;
    }
    #endregion
    
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
    
    #region Submit, Delete, Cancel Button with Animation

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            //Todo some animation.
            var isSuccess = await SubmitAction(UserAction);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Employee Saved!");
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
        Employee responseModel = null;
        bool result = false;
        _inputMode.User.AccountId = LoginUser.AccountId;
        _inputMode.User.DomainURL = _navigationManager.BaseUri;
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.Create}";
                responseModel = await _httpService.POST<Employee>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.Update}";
                responseModel = await _httpService.PUT<Employee>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Employee JSON : {_inputMode.ToJson()}");
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
            Utilities.SnackMessage(Snackbar, "Employee Deleted!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    #endregion

    #region Get Fake Data
    private async Task GetFakeData()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.Fake}";
        _inputMode = await _httpService.GET<Employee>(url);
        _loading = false;
        //throw new NotImplementedException();
    }
    

    #endregion
    
}