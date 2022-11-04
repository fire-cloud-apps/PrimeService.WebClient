using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Inventory.ListItems;

public partial class ProductTransactionList
{
    #region Variables
    [Inject] 
    ISnackbar Snackbar { get; set; }
    /// <summary>
    /// Product Id
    /// </summary>
    [Parameter]
    public string Id { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private string searchString = null;
    private IEnumerable<Model.ProductTransaction> _data = new List<Model.ProductTransaction>();
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.ProductTransaction> _groupDefinition = new()
    {
        GroupName = "Stock",
        Indentation = false,
        Expandable = true,
        Selector = (e) => e.Action
    };
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    private Model.Product _inputMode;
     
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion
        _inputMode = new Model.Product()
        {
            Category = new Model.ProductCategory(),
            TaxGroup = new Tax()
        };
        if (string.IsNullOrEmpty(Id))
        {
            //Add Mode
        }
        else
        {
            //This case always we should get the _Id.
            Console.WriteLine($"Product Id : {Id}");
            await GetModelDetails(Id);
            //Edit Mode.
        }
        _loading = false;
        StateHasChanged();
    }
    #region Get Model Details - Edit
    private async Task GetModelDetails(string id)
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.GetDetails}";
        url = string.Format(url, id);
        Utilities.ConsoleMessage($"URL {url}");
        _inputMode = await _httpService.GET<Model.Product>(url);
        _loading = false;
    }
    #endregion
    #endregion

   
    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.ProductTransaction> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = string.Empty;
    private string _searchField = "ProductId.Reason";
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.ProductTransaction>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.ProductTransaction>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }
    
    /// <summary>
    /// Do Ajax call to get 'ProductTransaction' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>Product Data.</returns>
    private async Task<ResponseData<Model.ProductTransaction>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductTransactionApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "TransactionDate" : state.SortLabel,
            SearchField = _searchField,
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D",
            FilterParams = new List<string>() { Id }
        };
        var responseModel = await _httpService.POST<ResponseData<Model.ProductTransaction>>(url, pageMetaData);
        return responseModel;
    }

    private void OnSearch(string text, string field = "Name")
    {
        _searchString = text;
        _searchField = field;
        _mudTable.ReloadServerData();//If we put Async, Loading progress bar is not closing.
        StateHasChanged();
    }
    #endregion
    
    #region Print the Transactions
    private async Task PrintIt(MouseEventArgs arg)
    {
        //to do some printing activity.
        //We can refer: https://github.com/Append-IT/Blazor.Printing
        //_navigationManager.NavigateTo("/Action/?Component=Product");
    }
    #endregion
}