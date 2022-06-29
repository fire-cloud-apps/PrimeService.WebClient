using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;
using FireCloud.WebClient.PrimeService.Service.QueryString;
using MongoDB.Bson;
using PrimeService.Model;

namespace FC.PrimeService.Shopping.Client.Component;

public partial class ClientComponent
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;

    public Model.Client _inputMode;
   
    /// <summary>
    /// Client unique Id to get load the data.
    /// </summary>
    [Parameter] public string? Id { get; set; } 
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = false;
    private IEnumerable<ClientType> _clientTypes = new List<ClientType>();
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    private UserAction UserAction;

    #endregion
    
    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        Utilities.ConsoleMessage($"Client Id: {Id}");
        if (string.IsNullOrEmpty(Id))
        {
            UserAction = UserAction.ADD;
            _inputMode = new Model.Client();
        }
        else
        {
            UserAction = UserAction.EDIT;
            await GetModelDetails(Id);
            
        }
        Utilities.ConsoleMessage($"Load Completed.");
        StateHasChanged();
    }
    #endregion
    
    #region Get Model Details - Edit
    private async Task GetModelDetails(string id)
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientApi.GetDetails}";
        url = string.Format(url, id);
        Utilities.ConsoleMessage($"URL {url}");
        _inputMode = await _httpService.GET<Model.Client>(url);
        _loading = false;
    }
    

    #endregion
    
    #region 'ClientType' Autocomplete - Search

    private async Task<IEnumerable<ClientType>> ClientType_SearchAsync(string value)
    {
        var responseData = await GetDataByBatch(value);
        _clientTypes = responseData.Items;
        return _clientTypes;
    }

    
    #region ClientType - AutoComplete Ajax call
    private async Task<ResponseData<ClientType>> GetDataByBatch(string searchValue)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientTypeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = "Title",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<ClientType>>(url, pageMetaData);
        return responseModel;
    }
    #endregion
    
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
                Utilities.SnackMessage(Snackbar, "Client Saved!");
            }
        }
        else
        {
            _outputJson = "Validation Error occured.";
        }
        Utilities.ConsoleMessage(_outputJson);
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        Model.Client responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientApi.Create}";
                responseModel = await _httpService.POST<Model.Client>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientApi.Update}";
                responseModel = await _httpService.PUT<Model.Client>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Client JSON : {_inputMode.ToJson()}");
        _processing = false;
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

    #region Get Fake Data
    private async Task GetFakeData()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientApi.Fake}";
        _inputMode = await _httpService.GET<Model.Client>(url);
        _loading = false;
        //throw new NotImplementedException();
    }
    

    #endregion

}