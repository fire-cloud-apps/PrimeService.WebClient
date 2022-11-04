using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Shop.ListItems;

public partial class ShopList
{
    #region Variables
    [Inject] 
    ISnackbar Snackbar { get; set; }
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Model.Sales> pagedData;
    private int totalItems;
    private string searchString = null;
    DateRange _dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);
    private DateRange SelectedDateRange
    {
        get
        {
            return _dateRange;
        }
        set
        {
            _dateRange = value;
            Console.WriteLine($"Selected Date Range - From :{_dateRange.Start} End : {_dateRange.End}");
            string dtRange = $"{_dateRange.Start}-{_dateRange.End}";
            Console.WriteLine($"Date Range - {dtRange}");
            OnSearch(dtRange, "Range");
        }
    }
    private User _loginUser;
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    #region Payment Selection

    private MudListItem _selectedPayment;
    private MudListItem SelectedPayment
    {
        get
        {
            return _selectedPayment;
        }
        set
        {
            _selectedPayment = value;
            if (_selectedPayment.Value.ToString() == "0")
            {
                OnSearch(string.Empty, "PaymentStatus");
            }
            else
            {
                OnSearch(_selectedPayment.Text, "PaymentStatus");
            }
            
            Console.WriteLine($"Selected Item Payment Value: {_selectedPayment.Value} - Text: { _selectedPayment.Text}");
        }
    }

    
    #endregion
    
    private IEnumerable<Model.Sales> _data = new List<Model.Sales>();
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.Sales> _groupDefinition = new()
    {
        GroupName = "Sales Status",
        Indentation = false,
        Expandable = true,
        Selector = (e) => e.Status
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call Initialized.
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion
        _loading = false;
        _loginUser = await _localStore.GetItemAsync<User>("user");
        Utilities.ConsoleMessage($"Login User {_loginUser.AccountId}");
        _loading = true;
        StateHasChanged();
    }
    #endregion
    
    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.Sales> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = string.Empty;
    private string _searchField = "Name";
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.Sales>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Sales>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }
    
    /// <summary>
    /// Do Ajax call to get 'Sales' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>Sales Data.</returns>
    private async Task<ResponseData<Model.Sales>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SalesApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Name" : state.SortLabel,
            SearchField = _searchField,
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.Sales>>(url, pageMetaData);
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
    
    #region Add Action
    private async Task AddAction(MouseEventArgs arg)
    {
        var url = $"/Sales?viewId=POList";
        //Navigate and open in new tab.
        await  JSRuntime.InvokeAsync<object>("open", 
            new object[2] { url, "_blank" });
    }
    #endregion

    #region Action Dialog
    private async Task AddProductCategory()
    {
        await InvokeDialog("_ProductCategory","Product Category", null);//Null indicates its an 'Add' Mode.
    }
    private async Task InvokeDialog(string parameter, string title, Model.ProductCategory model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<ProductCategoryDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    #endregion
   
}