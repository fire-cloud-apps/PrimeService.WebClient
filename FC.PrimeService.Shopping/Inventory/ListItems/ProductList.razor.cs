using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings.Forms;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Inventory.ListItems;

public partial class ProductList
{
    #region Variables
    [Inject] 
    ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private int totalItems;
    
    private User _loginUser;
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    #region Category Selection

    private MudListItem _selectedCategory;
    private MudListItem SelectedCategory
    {
        get
        {
            return _selectedCategory;
        }
        set
        {
            _selectedCategory = value;
            if (_selectedCategory.Value.ToString() == "0")
            {
                OnSearch(string.Empty, "Category.Id");
            }
            else
            {
                OnSearch(_selectedCategory.Value.ToString(), "Category.Id");
            }
            
            Console.WriteLine($"Selected Item Category Value: {_selectedCategory.Value} - Text: { _selectedCategory.Text}");
        }
    }

    
    #endregion
    
    IEnumerable<Model.Product> _data = new List<Model.Product>()
    {
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c50",
            Barcode = "52LL909LKLKD",
            Name = "Samsung Mobile M31",
            Notes = "Some Data",
            Quantity = 5,
            SellingPrice = 18000,
            Category = new Model.ProductCategory(){ CategoryName = "Mobile"}
        },
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c51",
            Barcode = "96LL909LKLKD",
            Name = "Samsung Mobile M32",
            Notes = "Some Data",
            Quantity = 15,
            SellingPrice = 19000,
            Category = new Model.ProductCategory(){ CategoryName = "Mobile"}
        },
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c52",
            Barcode = "85LL909LKLKD",
            Name = "Samsung Mobile M33",
            Notes = "Some Data",
            Quantity = 10,
            SellingPrice = 22800,
            Category = new Model.ProductCategory(){ CategoryName = "Mobile"}
        },
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c60",
            Barcode = "4759645KLLD",
            Name = "Lenovo L67",
            Notes = "Some Data",
            Quantity = 25,
            SellingPrice = 48500,
            Category = new Model.ProductCategory(){ CategoryName = "Laptop"}
        },

    };
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };

    private IEnumerable<Model.ProductCategory> _productCategories;
    private TableGroupDefinition<Model.Product> _groupDefinition = new()
    {
        GroupName = "Quantity",
        Indentation = false,
        Expandable = true,
        Selector = (e) => e.Quantity
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion

        _productCategories = await GetProductCategory();
        _loading = false;
        _loginUser = await _localStore.GetItemAsync<User>("user");
        Utilities.ConsoleMessage($"Login User {_loginUser.AccountId}");
        StateHasChanged();
    }
    #endregion

    #region Load Product Category
    
    private async Task<IEnumerable<Model.ProductCategory>> GetProductCategory()
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetProductCategory_ByBatch();
        #endregion
        
        return responseModel.Items;
    }
    
    /// <summary>
    /// Do Ajax call to get 'ProductCategory' Data
    /// </summary>
    /// <returns>ProductCategory Data.</returns>
    private async Task<ResponseData<Model.ProductCategory>> GetProductCategory_ByBatch()
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductCategoryApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = string.Empty,
            Page = 0,
            PageSize = 25,
            SortLabel = "CategoryName" ,
            SearchField = "CategoryName",
            SortDirection = "A" 
        };
        var responseModel = await _httpService.POST<ResponseData<Model.ProductCategory>>(url, pageMetaData);
        return responseModel;
    }
    
    #endregion
    
    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.Product> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = string.Empty;
    private string _searchField = "Name";
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.Product>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Product>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }
    
    /// <summary>
    /// Do Ajax call to get 'Product' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>Product Data.</returns>
    private async Task<ResponseData<Model.Product>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Name" : state.SortLabel,
            SearchField = _searchField,
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.Product>>(url, pageMetaData);
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
    
    #region Add Action Product
    private async Task AddAction(MouseEventArgs arg)
    {
        _navigationManager.NavigateTo("/Action/?Component=Product");
    }
    #endregion

    #region Add - Product Category Dialog 

    private async Task AddProductCategory()
    {
        await InvokeDialog("ProductCategory","Add Product Category", null);//Null indicates its an 'Add' Mode.
    }
    private async Task InvokeDialog(string parameter, string title, Model.ProductCategory model)
    {
        var parameters = new DialogParameters
        {
            ["ProductCategory"] = model,
            ["UserAction"] =  UserAction.ADD as object,
            ["Title"] = title
        };
        var dialog = DialogService.Show<ProductCategoryDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
        _productCategories = await GetProductCategory();
        StateHasChanged();
    }


    #endregion

}