using System.Text.Json;
using FC.PrimeService.Shopping.Inventory.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Inventory.Component;

public partial class ProductComponent
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private string _display = "d-none";//Display 'none' during loading.
    private Model.Product _inputMode;
   
    /// <summary>
    /// Client unique Id to get load the data.
    /// </summary>
    [Parameter] public string? Id { get; set; } 
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private bool _editToggle = false;
    public IEnumerable<Model.ProductCategory> _productCategory = new List<Model.ProductCategory>();
    private User _loginUser;

    private IEnumerable<Model.ProductTransaction> _productTransactions =
        new List<Model.ProductTransaction>();

    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    private UserAction UserAction;
        
    #endregion
    
    #region Load Async
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        
        Utilities.ConsoleMessage($"Client Id: {Id}");
        await SetGlobalConfiguration();
        
        if (string.IsNullOrEmpty(Id))
        {
            UserAction = UserAction.ADD;
            _inputMode = new Model.Product()
            {
                Category = new Model.ProductCategory(),
                TaxGroup = new Tax()
            };
            _editToggle = true;
            _display = "d-flex";
            _isReadOnly = false;
            
        }
        else
        {
            UserAction = UserAction.EDIT;
            await GetModelDetails(Id);
            _display = "d-none";
            _editToggle = false;
            _productTransactions = await ProductTransaction_SearchAsync(Id);
        }
        _loginUser = await _localStore.GetItemAsync<User>("user");
        Utilities.ConsoleMessage($"Load Completed.");
        _loading = false;
        StateHasChanged();
    }
    
    /// <summary>
    /// Set Global Configuration
    /// </summary>
    private async Task SetGlobalConfiguration()
    {
        if (GlobalConfig.LoginUser == null)
        {
            Utilities.ConsoleMessage("GlobalConfig.LoginUser Is 'null'");
            GlobalConfig.LoginUser = await _localStore.GetItemAsync<AuditUser>("LoginUser");
        }
        else
        {
            Utilities.ConsoleMessage($"Global User {GlobalConfig.LoginUser.ToJson()}");
        }
    }

    #endregion
    
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

    #region ProductTransaction Search - Last 10 Transaction

    private async Task<IEnumerable<Model.ProductTransaction>> ProductTransaction_SearchAsync(string value)
    {
        var responseData = await GetProdTransaction_ByBatch(value);
        _productTransactions = responseData.Items;
        return _productTransactions;
    }
    
    #region ProductTransaction - Ajax call
    private async Task<ResponseData<Model.ProductTransaction>> GetProdTransaction_ByBatch(string searchValue)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductTransactionApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? "0" : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = "ProductId",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.ProductTransaction>>
            (url, pageMetaData);
        return responseModel;
    }
    #endregion
    
    #endregion
    
    #region Product Category Search - Autocomplete

    private async Task<IEnumerable<Model.ProductCategory>> ProductCategory_SearchAsync(string value)
    {
        var responseData = await GetDataByBatch(value);
        _productCategory = responseData.Items;
        return _productCategory;
    }
    
    #region ClientType - AutoComplete Ajax call
    private async Task<ResponseData<Model.ProductCategory>> GetDataByBatch(string searchValue)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductCategoryApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = "Title",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.ProductCategory>>(url, pageMetaData);
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
            var isSuccess = await SubmitAction(UserAction);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Product Saved!");
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
        Model.Product responseModel = null;
        bool result = false;
        Utilities.ConsoleMessage($"Product JSON : {_inputMode.ToJson()}");
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.Create}";
                responseModel = await _httpService.POST<Model.Product>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.Update}";
                responseModel = await _httpService.PUT<Model.Product>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        
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

    #region Fake Data

    private async Task GetFakeData()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.Fake}";
        _inputMode = await _httpService.GET<Model.Product>(url);
        _loading = false;
    }

    #endregion

    #region 'Tax' Search - Autocomplete

    private IEnumerable<Tax> _taxList = new List<Tax>();
  
    private async Task<IEnumerable<Tax>> Tax_SearchAsync(string value)
    {
        var responseData = await GetTaxDataByBatch(value);
        _taxList = responseData.Items;
        return _taxList;
    }

    
    #region Tax - AutoComplete Ajax call
    private async Task<ResponseData<Tax>> GetTaxDataByBatch(string searchValue)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TaxApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = "Title",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<Tax>>(url, pageMetaData);
        return responseModel;
    }
    #endregion

    #endregion
    
    #region Tool Bar Action Dialogs
    
    private async Task EditToggle(bool toggled)
    {
        _isReadOnly = !toggled;
        Console.WriteLine(_isReadOnly);
        if (_isReadOnly)
        {
            _display = "d-none";
        }
        else
        {
            _display = "d-flex";
        }
        //d-flex or d-none
    }
    private async Task AddStock(MouseEventArgs arg)
    {
        await InvokeDialog("Product","Product", _inputMode, ActionType.AddStock);
    }

    private async Task ReduceStock(MouseEventArgs arg)
    {
        await InvokeDialog("Product","Product", _inputMode, ActionType.ReduceStock);
    }
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    private async Task InvokeDialog(string parameter, string title, Model.Product model, ActionType actionType)
    {
        var parameters = new DialogParameters
        {
            [parameter] = model,
            ["User"] = _loginUser
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        IDialogReference dialog;
        if (actionType == ActionType.AddStock)
        {
            dialog = DialogService.Show<AddStockDialog>(title, parameters, _dialogOptions);
        }
        else
        {
            dialog = DialogService.Show<DecreaseStockDialog>(title, parameters, _dialogOptions);
        }
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
        await GetModelDetails(Id);
        _productTransactions = await ProductTransaction_SearchAsync(Id);
        await Task.Delay(500);
        StateHasChanged();
    }

    enum ActionType
    {
        AddStock,
        ReduceStock
    }    

    #endregion

    #region Load More 'Product' Transaction
    private async Task LoadMore()
    {
        _navigationManager.NavigateTo($"/Inventory?viewId=PT&Id={_inputMode.Id}");
    }
    #endregion
    
    #region Validation
    private string ValidateQuntity(int qty)
    {
        if (_inputMode.MinQuantity == 0) return null;
        if (_inputMode.MaxQuantity <= _inputMode.MinQuantity)
            return "Maximum Quantity should be greater then 'Min' Quantity";
        else
            return null;
    }
    
    private string ValidatePrice(double price)
    {
        if (_inputMode.SupplierPrice == 0) return null;
        if (_inputMode.SellingPrice <= _inputMode.SupplierPrice)
            return $"Selling Price '{_inputMode.SellingPrice}' cannot be lesser then or Equal to Supplier Price '{_inputMode.SupplierPrice}'.";
        else
            return null;
    }

    #endregion

    private async Task PrintProduct()
    {
        object[] args = new object[] { "SomeInfo" };
        var res = await JSRuntime.InvokeAsync<bool>("printProduct",args);
        Utilities.ConsoleMessage($"Invoked Printing Method");
    }
}