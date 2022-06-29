using System.Text.Json;
using FC.Common.Domain;
using FC.PrimeService.Common.Settings.Dialog;
using FC.PrimeService.Shopping.Client.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Client.ListItems;

public partial class ClientList
{
    
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    private bool _loading = false;
    private Model.Client _inputMode;
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private User _loginUser;
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    #endregion
    #region Variables
   
    
    private TableGroupDefinition<Model.Client> _groupDefinition = new()
    {
        GroupName = "Type",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.Type.Title
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion
        _loading = false;
        _loginUser = await _localStore.GetItemAsync<User>("user");
        Utilities.ConsoleMessage($"Login User {_loginUser.AccountId}");
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.Client> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = null;
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.Client>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Client>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }

    /// <summary>
    /// Do Ajax call to get 'Client' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>WorkLocation Data.</returns>
    private async Task<ResponseData<Model.Client>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(_searchString)) ? string.Empty : _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Name" : state.SortLabel,
            SearchField = "Mobile",
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.Client>>(url, pageMetaData);
        return responseModel;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _mudTable.ReloadServerData();//If we put Async, Loading progress bar is not closing.
        StateHasChanged();
    }
    #endregion
    
    #region Add Action

    private async Task AddAction(MouseEventArgs arg)
    {
        var url = $"/Action?Component=Client&Id=";
        //Navigate and open in new tab.
        await JSRuntime.InvokeAsync<object>("open",
            new object[2] { url, "_blank" });
        OnSearch(string.Empty);
    }

    #endregion
    
}