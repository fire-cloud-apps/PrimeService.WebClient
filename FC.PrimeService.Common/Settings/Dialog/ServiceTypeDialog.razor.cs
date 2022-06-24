using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class ServiceTypeDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    
    #region Dialog Parameters
    [Parameter] public ServiceType ServiceType { get; set; } 
    [Parameter] public string Title { get; set; }
    [Parameter] public UserAction UserAction { get; set; }
    #endregion
    private bool _processing = false;
    MudForm form;
    private ServiceType _inputMode;
    string _outputJson;
    bool success;
    private bool _isReadOnly = false;

    public IEnumerable<ServiceCategory> _serviceCategory = new List<ServiceCategory>();
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
        Utilities.ConsoleMessage($"Service Type - User Action : {UserAction}");
        if (UserAction == UserAction.ADD)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new ServiceType();//Initializes an empty object.
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = ServiceType;
        }

        _loading = false;
        StateHasChanged();
    }
    #endregion
    
    
    #region ServiceCategory Search - Autocomplete

    private async Task<IEnumerable<ServiceCategory>> ServiceCategory_SearchAsync(string value)
    {
        var responseData = await GetDataByBatch(value);
        _serviceCategory = responseData.Items;
        return _serviceCategory;
    }
    #endregion
    
    #region ServiceCategory - AutoComplete Ajax call
    private async Task<ResponseData<ServiceCategory>> GetDataByBatch(string searchValue)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceCategoryApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = "Title",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<ServiceCategory>>(url, pageMetaData);
        return responseModel;
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
                Utilities.SnackMessage(Snackbar, "ServiceType Saved!");
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
        ServiceType responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Create}";
                responseModel = await _httpService.POST<ServiceType>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Update}";
                responseModel = await _httpService.PUT<ServiceType>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"ServiceType JSON : {_inputMode.ToJson()}");
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
            Utilities.SnackMessage(Snackbar, "ServiceType Deleted!", Severity.Warning);
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
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Fake}";
        _inputMode = await _httpService.GET<ServiceType>(url);
        _loading = false;
    }
    #endregion
    
    
}