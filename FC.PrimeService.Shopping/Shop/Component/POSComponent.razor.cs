using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Xml.Schema;
using FC.PrimeService.Shopping.Client.Dialog;
using FC.PrimeService.Shopping.Inventory.Dialog;
using FC.PrimeService.Shopping.Shop.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Win32.SafeHandles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Payments;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Shop.Component;

public partial class POSComponent
{
    
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private string _display = "d-flex";//"d-none";//Display 'none' during loading.
    private Model.Sales _inputMode;

    private string _termsCondition = "1. Goods once sold will not be taken back or Exchanged \n" +
                                     "2. A late fee of 5% of due amount will be added for delayed payments";

    private bool isTaxOpen;
    
   
    /// <summary>
    /// Client unique Id to get load the data.
    /// </summary>
    [Parameter] public string? Id { get; set; } 
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = false;
    private bool _editToggle = false;
    private Model.SalesStatus _selectedStatus { get; set; } = Model.SalesStatus.Confirmed;
    
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
        _httpService = new HttpService(_httpClient, _navigationManager, 
            _localStore, _configuration, Snackbar);
        await GetPaymentMethods_ByBatch();
         
        if (string.IsNullOrEmpty(Id))
        {
            UserAction = UserAction.ADD;
            var billNo = await GenerateBillNo();
            var responseData = await Utilities.GetClients(_appSettings, _httpService, "00-000");
             var guestClient = responseData.Items.FirstOrDefault();
            _inputMode = new Model.Sales()
            {
                BillNumber = billNo,
                TransactionDate = DateTime.Now,
                Status = Model.SalesStatus.Draft,
                BilledBy = await Utilities.GetLoginUser(_localStore),
                Client = guestClient,
                Products = _salesTransaction,
                PaymentAccount = await Utilities.GetDefault_PaymentAccount(_appSettings, _httpService),
                PaymentStatus = Model.PaymentStatus.Paid,
                PaymentMethod = await Utilities.GetDefault_PaymentMethods(_appSettings, _httpService)
            };
            _editToggle = true;
            //_display = "d-flex";
            _isReadOnly = false;
        }
        else
        {
            UserAction = UserAction.EDIT;
            _inputMode = await Get_Sales(Id);
            _salesTransaction = _inputMode.Products;
            Console.WriteLine($"Edit Data {_inputMode.ToJson()}");
            //_display = "d-none";
            _editToggle = false;
            Console.WriteLine($"Id: {Id}");
            CalculateSales();
            //TODO: Get the details from 'Sales'
        }
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Get Default Payment Account

    private async Task<Model.Sales> Get_Sales(string salesId)
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SalesApi.GetDetails}";
        url = string.Format(url, salesId);
        Utilities.ConsoleMessage($"Get Sales Data: {url}");
        var data = await _httpService.GET<Model.Sales>(url);
        _loading = false;
        return data;
    }
    private async Task<PaymentTags> GetDefault_PaymentAccount()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentTagsApi.GetDefault}";
        Utilities.ConsoleMessage($"Default Account No: {url}");
        var accounts = await _httpService.GET<PaymentTags>(url);
        _loading = false;
        return accounts;
    }

    #endregion
    
    #region Generate Sales Unique Id
    private async Task<string> GenerateBillNo()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SalesApi.GenerateBillNo}";
        Utilities.ConsoleMessage($"Generate Bill No: {url}");
        var serialNumber = await _httpService.GET<SerialNumber>(url);
        _loading = false;
        return serialNumber.Preview;
    }
    #endregion
    
    #region Client Search - Autocomplete

    private IEnumerable<Model.Client> _clients = new List<Model.Client>();
    
    private async Task<IEnumerable<Model.Client>> Client_SearchAsync(string value)
    {
        var responseData = await Utilities.GetClients(_appSettings, _httpService, value);
        _clients = responseData.Items;
        Console.WriteLine($"Find Client : '{value}'" );
        return _clients;
    }
    
    #endregion
    
    #region Product Search & BarCode Search - Autocomplete
    private Model.Product _selectedProduct = new Model.Product();
    private IEnumerable<Model.Product> _productList = new List<Model.Product>();
    
    private async Task<IEnumerable<Model.Product>> Product_SearchAsync(string value)
    {
        var responseData = await GetProduct_DataByBatch(value);
        _productList = responseData.Items;
        Utilities.ConsoleMessage($"Product Count : {_productList.Count()}");
        return _productList;
    }
    
    #region Product - AutoComplete Ajax call
    private async Task<ResponseData<Model.Product>> GetProduct_DataByBatch
    (string searchValue, string searchField ="Name")
    {
        Utilities.ConsoleMessage("3. GetProduct_DataByBatch");
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.Product>>(url, pageMetaData);
        return responseModel;
    }
    #endregion
    
    private async Task ProductSelected_Changed(Model.Product selectedProduct)
    {
        if (selectedProduct != null)
        {
            Console.WriteLine($"Selected Product - Name: {selectedProduct.Name} | BarCode: {selectedProduct.Barcode}");
            AddAndCalculate(selectedProduct);
        }
    }
    #endregion

    #region Find by BarCode
    private string? _barCode = string.Empty;
    private MudTextField<string> _barcodeMudTextField;
    private string? GetBarCode
    {
        get => _barCode;
        set
        {
            _barCode = value;
            if(string.IsNullOrEmpty(_barCode)) return;
            
            Utilities.ConsoleMessage("1. GetBarCode");
            AddProductToSales(_barCode);
            Utilities.ConsoleMessage($"Bind BarCode: {_barCode}");
        }
    }
    
    /// <summary>
    /// Search the exact value of the barcode.
    /// </summary>
    /// <param name="value">BarCode value</param>
    /// <returns>returns Product</returns>
    private async Task<IEnumerable<Model.Product>> ProductBar_SearchAsync(string value)
    {
        Utilities.ConsoleMessage("2. ProductBar_SearchAsync");
        var responseData = await GetProduct_DataByBatch(value, "Barcode");
        _productList = responseData.Items;
        return _productList;
    }

    
    private async Task BarCode_ValueChanged(string arg)
    {
        Console.WriteLine($"BarCode : {arg}");
        Console.WriteLine($"Bind BarCode: {_barCode}");
    }
    

    #endregion
    
    #region Tool Bar Action Dialogs
    private async Task OpenExpandPOS()
    {
        var url = $"/SalesFullView?Id={_inputMode.Id}";
        //Navigate and open in new tab.
        await  JSRuntime.InvokeAsync<object>("open", 
            new object[2] { url, "_blank" });
    }
    
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
    
    #endregion
    
    private async Task LoadMore()
    {
        _navigationManager.NavigateTo($"/Sales?viewId=POS&Id={_inputMode.Id}");
    }

    #region Load Payment Methods 

    /// <summary>
    /// Assume that this comes from Ajax call.
    /// </summary>
    private IEnumerable<PaymentMethods> _paymentMethods = new List<PaymentMethods>();
    
    private async Task<ResponseData<PaymentMethods>> GetPaymentMethods_ByBatch()
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentMethodsApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = string.Empty,
            Page = 0,
            PageSize = 15,
            SortLabel = "Title",
            SearchField = "Title",
            SortDirection = "A"
        };
        var responseModel = await _httpService.POST<ResponseData<PaymentMethods>>(url, pageMetaData);
        _paymentMethods = responseModel.Items; 
        return responseModel;
    }

    #endregion
    
    #region Sales Transaction
    
    private string _currency = "₹";
    
    private IList<Model.PurchasedProduct> _salesTransaction = new List<Model.PurchasedProduct>();
    private async Task AddProductToSales(string barcode)
    {
        var products = await ProductBar_SearchAsync(barcode);
        var product = products?.FirstOrDefault();
        if (product == null)
        {
            Utilities.ConsoleMessage("Product Not found");
            return;
        }
        Utilities.ConsoleMessage("4.AddProductToSales");
        AddAndCalculate(product);
        _barCode = string.Empty;
    }

    void AddAndCalculate(Model.Product product)
    {
        AddProductForSales(product);//Adds the product to the "_salesTransaction"
       
        CalculateSales();//Uses '_salesTransaction" to calculate the discount/Tax/SubTotal/Additional Charges etc.
    }
    
    private void AddProductForSales(Model.Product product)
    {
        #region Add Product for Sales
        Model.PurchasedProduct purchased = new Model.PurchasedProduct()
        {
            ProductId = product?.Id,
            Discount = product.Discount,
            Price = Math.Round(product.SellingPrice, 2),
            Quantity = 1,
            ProductName = product.Name,
            AppliedTax = product.TaxGroup,
        };
        //SubTotal - Calculated at the method 'CalculateSales' 
        //If we calculate here then it will wont calculate total quantity. also all calculations should be in one place.
        var isProductExists = _salesTransaction.FirstOrDefault(prd => prd.ProductId == purchased.ProductId);
        string msg = string.Empty;
        
        if (isProductExists == null)
        {
            _salesTransaction.Add(purchased); //If product does not exists in the invoice list add it.
            msg = string.Concat(purchased.ProductName, " Added ", " 1 Quantity.");
        }
        else
        {
            isProductExists.Quantity++; //Add the Quantity if the product already exists
            msg = string.Concat(purchased.ProductName, " Added ", isProductExists.Quantity, " Quantity.");
        }
        Snackbar.Add(msg, Severity.Success);
        #endregion
    }


    private string _subTotalHelpText = string.Empty;
    /// <summary>
    /// SubTotal Price, Tax, Discount, Additional Charges were Calculated.
    /// </summary>
    private void CalculateSales()
    {
        #region Price, Tax, Discount Calculation
        int totQty = 0; //Total Item (Qty)
        double totDiscount = 0d;
        double tax = 0;
        double total = 0;
        
        StringBuilder sbNotes = new StringBuilder();
        foreach (var item in _salesTransaction)
        {
            //Calculates Tax for individual product
            tax = tax + ((item.AppliedTax.TaxRate / 100) * item.Price) * item.Quantity;
            
            //Calculate Total Quantity
            totQty = totQty + item.Quantity;
            item.SubTotal = item.Price * item.Quantity;
            total = total + item.SubTotal;
            
            //Calculate Discount
            item.DiscountPrice = Math.Round((item.Discount / 100) * item.SubTotal, 2);
            //totDiscount = item.DiscountPrice * item.Quantity;
            totDiscount = totDiscount + item.DiscountPrice;// * item.Quantity;
            sbNotes.AppendLine(string.Concat(item.ProductName, " - ", item.Quantity));
            Utilities.ConsoleMessage("");
        }
        //Notes
        _inputMode.Notes = sbNotes.ToString();
        
        //Total Quantity bought by customer.
        _inputMode.TotalQuantity = totQty;
        
        //Discount calculation. Before applying Tax/Additional cost we should calculate discount.
        double beforeDiscount = total;
        total = total - totDiscount;
        _inputMode.TotalDiscount = Math.Round(totDiscount,2);
        
        //Item Total/Sub Total of all items
        _inputMode.SubTotal = Math.Round(total, 2);
        _subTotalHelpText = $"Total Cost = SubTotal - Discount eg. {_inputMode.SubTotal}={beforeDiscount} - {totDiscount}";
        
        //Additional Cost Calculation
        double grandTotal = total + _inputMode.AdditonalCost;
        
        //Total Tax
        tax = Math.Round(tax, 2); //Show only '2' decimal points.
        _inputMode.TotalTax = tax;
        
        //Grand Total
        grandTotal = grandTotal + tax; //Tax Added in th GrandTotal
        grandTotal = Math.Round(grandTotal, 2); //Show only '2' decimal points.
        _inputMode.GrandTotal = grandTotal;
        #endregion
    }
    
    #endregion

    #region Sales Item Adjust stock/price/discount any action in the Dialog

    private async Task SalesItemProductAction(Model.PurchasedProduct purchasedProduct)
    {
        await InvokeDialog("_PurchasedProduct", "Edit Product", purchasedProduct);
        Console.WriteLine($"Product: { purchasedProduct.ProductName}");
    }

    private string state;
    private async Task RemoveSalesItem(Model.PurchasedProduct purchasedProduct)
    {
        bool? result = await mbox.Show();
        state = result == null ? "Cancelled" : "Deleted!";
        int? itemIndex = 0;
        if (state == "Deleted!")
        {
            foreach (Model.PurchasedProduct product in _salesTransaction)
            {
                if (product.ProductId == purchasedProduct.ProductId)
                {
                    itemIndex =  _salesTransaction.IndexOf(product);
                    break;
                }
            }
            _salesTransaction.RemoveAt(itemIndex.Value);
            CalculateSales();
            Snackbar.Add("Item Removed", Severity.Error);
        }
        StateHasChanged();
    }
    MudMessageBox mbox { get; set; }
    
    #endregion

    private async Task AdditionalCost_LostFocus()
    {
        CalculateSales();
    }

    #region Sales Content Edit Dialog - During sales
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Medium,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    private async Task InvokeDialog(string parameter, string title, Model.PurchasedProduct model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        IDialogReference dialog;

        dialog = DialogService.Show<POSSalesItemDialog>(title, parameters, _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            CalculateSales();
        }
        
    }
    #endregion
    
    private async Task AddClientDialog()
    {
        var parameters = new DialogParameters
            { ["Client"] = null }; 
        IDialogReference dialog;
        
        dialog = DialogService.Show<ClientDialog>(
            string.Empty, 
            parameters,
            _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    
    #region Submit, Delete, Cancel Button with Animation

    private async Task PayLater()
    {
        _inputMode.PaymentStatus = Model.PaymentStatus.Pending;
        await Submit();
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            //Custom Validation
            Utilities.ConsoleMessage($"Count {_inputMode.Products.Count}");
            if  (_inputMode.Products?.Count <= 0)
            {
                Utilities.SnackMessage(Snackbar, "No Product Selected", Severity.Error);
                return;
            }
            
            var isSuccess = await SubmitAction(UserAction);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Sales Saved!");
                await Task.Delay(500);
                Utilities.ConsoleMessage("Reload initiated.");
                //windowReload
                await JSRuntime.InvokeAsync<bool>("windowReload",null);
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
        _loading = true;
            
        string url = string.Empty;
        Model.Sales responseModel = null;
        bool result = false;
        
        switch (action)
        {
            case UserAction.ADD:
                _inputMode.Status = Model.SalesStatus.Confirmed;
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SalesApi.Create}";
                responseModel = await _httpService.POST<Model.Sales>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SalesApi.Update}";
                responseModel = await _httpService.PUT<Model.Sales>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.SalesApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Sales JSON : {_inputMode.ToJson()}");
        _processing = false;
        _loading = false;
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

    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }

    #endregion
}