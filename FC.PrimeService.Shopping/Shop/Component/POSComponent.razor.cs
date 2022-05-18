using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using FC.PrimeService.Shopping.Inventory.Dialog;
using FC.PrimeService.Shopping.Shop.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Win32.SafeHandles;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Payments;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Shop.Component;

public partial class POSComponent
{
    
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private string _display = "d-none";//Display 'none' during loading.
    public Model.Sales _inputMode;

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
    private bool _isReadOnly = true;
    private bool _editToggle = false;
    
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        //for now it is filled as Static value
        _inputMode = new Model.Sales()
        {
            Id = "6270d1cce5452d9169e86c50",
            Client = new Model.Client()
            {
                Name = "Guest", Mobile = "1234567890"
            },
            PaymentMethod = new PaymentMethods() { Id = "125", Title = "Cash" }
        };
        
        Console.WriteLine($"Id Received: {Id}"); //Use this id and get the values from 'API'
        if (Id == null)
        {
            Console.WriteLine("Add Mode");
            _editToggle = true;
            _display = "d-flex";
            _isReadOnly = false;
        }
        else
        {
            Console.WriteLine("Edit Mode");
            _display = "d-none";
            _editToggle = false;
            //Do ajax call and assign it to _inputMode
        }
        _loading = false;
        StateHasChanged();
    }
    
    #region Submit Button with Animation
    async Task ProcessSomething()
    {
        _processing = true;
        await Task.Delay(2000);
        _processing = false;
    }
    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // //Todo some animation.
            await ProcessSomething();

            //Do server actions.
            _outputJson = JsonSerializer.Serialize(_inputMode);

            //Success Message
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            //Snackbar.Configuration.VisibleStateDuration  = 2000;
            //Can also be done as global configuration. Ref:
            //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
            Snackbar.Add("Submited!", Severity.Success);
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine(_outputJson);
        }
    }

    #endregion

    #region Fake Data

    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }

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
    
    
  

    #endregion

    #region Client Search - Autocomplete
    public List<Model.Client> _clients = new List<Model.Client>()
    {
        new Model.Client() { Name = "SRG", Mobile = "8596963"},
        new Model.Client() { Name = "Alam", Mobile = "96936"},
        new Model.Client() { Name = "Pritish", Mobile = "74512"},
        new Model.Client() { Name = "Joshmitha", Mobile = "556326"},
        
    };
    private async Task<IEnumerable<Model.Client>> Client_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);
        Console.WriteLine($"Find Client : '{value}'" );
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _clients;
        }
        var result = _clients.Where(x => x.Mobile.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        if (result.FirstOrDefault() == null)
        {
            result = new List<Model.Client>()
            {
                new Model.Client() { Name = "Customer Not found", Mobile = "" }
            };
        }
        return result;
    }

    #endregion
    
    #region Product Search & BarCode Search - Autocomplete

    private Model.Product _selectedProduct = new Model.Product();
    public List<Model.Product> _productList = new List<Model.Product>()
    {
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c50",
            Barcode = "0002671908",
            Name = "Samsung Mobile M31",
            Notes = "Some Data",
            Discount = 0.5d,
            Quantity = 5,
            SellingPrice = 100,
            Category = new Model.ProductCategory(){ CategoryName = "Mobile"}
        },
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c51",
            Barcode = "0002687419",
            Name = "Samsung Mobile M32",
            Notes = "Some Data",
            Discount = 0.8d,
            Quantity = 15,
            SellingPrice = 200,
            Category = new Model.ProductCategory(){ CategoryName = "Mobile"}
        },
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c52",
            Barcode = "0002671930" ,
            Name = "Samsung Mobile M33",
            Notes = "Some Data",
            Discount = 5.0d,
            Quantity = 10,
            SellingPrice = 300,
            Category = new Model.ProductCategory(){ CategoryName = "Mobile"}
        },
        new Model.Product()
        {
            Id = "6270d1cce5452d9169e86c60",
            Barcode = "0002671901" ,
            Name = "Lenovo L67",
            Notes = "Some Data",
            Discount = 6.5d,
            Quantity = 25,
            SellingPrice = 500,
            Category = new Model.ProductCategory(){ CategoryName = "Laptop"}
        },
        
    }; // In reality it should come from API.
    
    private async Task<IEnumerable<Model.Product>> Product_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _productList;
        }
        return _productList.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task ProductSelected_Changed(Model.Product selectedProduct)
    {
        if (selectedProduct != null)
        {
            Console.WriteLine($"Selected Product - Name: {selectedProduct.Name} | BarCode: {selectedProduct.Barcode}");
            AddAndCalculate(selectedProduct);
        }
        
    }
    
    /// <summary>
    /// Search the exact value of the barcode.
    /// </summary>
    /// <param name="value">BarCode value</param>
    /// <returns>returns Product</returns>
    private async Task<IEnumerable<Model.Product>> ProductBar_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _productList;
        }
        return _productList.Where(x => x.Barcode == value);
    }

    private string? _barCode = string.Empty;
    private MudTextField<string> _barcodeMudTextField;
    private string? GetBarCode
    {
        get => _barCode;
        set
        {
            _barCode = value;
            AddProductToSales(_barCode);
            Console.WriteLine($"Bind BarCode: {_barCode}");
        }
    }
    private async Task BarCode_ValueChanged(string arg)
    {
        //ValueChanged="BarCode_ValueChanged"
        Console.WriteLine($"BarCode : {arg}");
        Console.WriteLine($"Bind BarCode: {_barCode}");
    }

    #endregion
    
    #region Tax Search - Autocomplete
    public List<Tax> _taxList = new List<Tax>()
    {
        new Tax()
        {
            Title = "0.00% - No Tax",
            TaxRate = 0.0f,
            Description = "No tax applied for the goods."
        },
        new Tax()
        {
            Title = "0.25% - Tax",
            TaxRate = 0.25f,
            Description = "Cut and semi-polished stones are included under this tax slab."
        },
        new Tax()
        {
            Title = "5% - Tax",
            TaxRate = 5.0f,
            Description = "Household necessities such as edible oil, sugar, spices, tea, and coffee (except instant) are included. Coal, Mishti/Mithai (Indian Sweets) and Life-saving drugs are also covered under this GST slab."
        },
        new Tax()
        {
            Title = "12% - Tax",
            TaxRate = 12.0f,
            Description = "This includes computers and processed food."
        },
        new Tax()
        {
            Title = "18% - Tax",
            TaxRate = 18.0f,
            Description = "Hair oil, toothpaste and soaps, capital goods and industrial intermediaries are covered in this slab."
        },
        new Tax()
        {
            Title = "28% - Tax",
            TaxRate = 28.0f,
            Description = "Luxury items such as small cars, consumer durables like AC and Refrigerators, premium cars, cigarettes and aerated drinks, High-end motorcycles are included here."
        },
        
    }; // In reality it should come from API.
    private async Task<IEnumerable<Tax>> Tax_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _taxList;
        }
        return _taxList.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion
    
    private async Task LoadMore()
    {
        _navigationManager.NavigateTo($"/Sales?viewId=POS&Id={_inputMode.Id}");
    }

    #region Payment Methods

    /// <summary>
    /// Assume that this comes from Ajax call.
    /// </summary>
    private IList<PaymentMethods> _paymentMethods = new List<PaymentMethods>()
    {
        new PaymentMethods()
        {
            Id = "123",
            Title = "Cash"
        },
        new PaymentMethods()
        {
            Id = "452",
            Title = "Card"
        },
        new PaymentMethods()
        {
            Id = "7896",
            Title = "QR"
        },
        new PaymentMethods()
        {
            Id = "969",
            Title = "Cashless"
        },
    };
    

    #endregion
    
    #region Sales Transaction

    
    private string _currency = "₹";
    
    private List<Model.PurchasedProduct> _salesTransaction = new List<Model.PurchasedProduct>();
    private void AddProductToSales(string barcode)
    {
        var product = _productList?.Where(code => code.Barcode == barcode).FirstOrDefault();
        if (product == null) return;
        AddAndCalculate(product);
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
            Price = product.SellingPrice,
            Quantity = 1,
            ProductName = product.Name,
            AppliedTax = _taxList[1],
        };
        //SubTotal - Calculated at the method 'CalculateSales' 
        //If we calculate here then it will wont calculate total quantity. also all calculations should be in one place.
        var isProductExists = _salesTransaction.FirstOrDefault(prd => prd.ProductId == purchased.ProductId);

        if (isProductExists == null)
        {
            _salesTransaction.Add(purchased); //If product does not exists in the invoice list add it.
        }
        else
        {
            isProductExists.Quantity++; //Add the Quantity if the product already exists
        }
        #endregion

    }


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
        foreach (var item in _salesTransaction)
        {
            tax = tax + (item.AppliedTax.TaxRate / 100) * item.Price;
            //Calculate Total Quantity
            totQty = totQty + item.Quantity;
            item.SubTotal = item.Price * item.Quantity;
            total = total + item.SubTotal;
            //Calculate Discount
            item.DiscountPrice = Math.Round((item.Discount / 100) * item.SubTotal, 2);
            totDiscount = item.DiscountPrice;
        }
        //Total Quantity bought by customer.
        _inputMode.TotalQuantity = totQty;
        //Discount calculation. Before applying Tax/Additional cost we should calculate discount.
        total = total - totDiscount;
        _inputMode.TotalDiscount = totDiscount;
        
        //Item Total/Sub Total of all items
        _inputMode.SubTotal = total;
        
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

    public List<Model.PurchasedProduct> _purchasedProduct = new List<Model.PurchasedProduct>()
    {
        new Model.PurchasedProduct()
        {
            ProductId = "6270d1cce5452d9169e86c50",
            ProductName = "Samsung Mobile M31",
            Quantity = 2,
            Discount = 2,
            AppliedTax = new Tax()
            {
                TaxRate = 5,
                Description = "5% Tax",
                Title = "5% Tax"
            },
            Price = 21568,
        },
        new Model.PurchasedProduct()
        {
            ProductId = "6270d1cce5452d9169e86c511",
            ProductName = "Samsung Mobile M21",
            Quantity = 1,
            Discount = 2,
            AppliedTax = new Tax()
            {
                TaxRate = 5,
                Description = "5% Tax",
                Title = "5% Tax"
            },
            Price = 16060,
        },
        new Model.PurchasedProduct()
        {
            ProductId = "6270d1cce5452d9169e86c600",
            ProductName = "Samsung Mobile M11",
            Quantity = 1,
            Discount = 2,
            AppliedTax = new Tax()
            {
                TaxRate = 18,
                Description = "18% Tax",
                Title = "18% Tax"
            },
            Price = 10060,
        },
    }; // In reality it should come from API.
    

    #endregion


    #region Sales Item Adjust stock/price/discount any action in the Dialog

    private async Task SalesItemProductAction(Model.PurchasedProduct purchasedProduct)
    {
        await InvokeDialog("_PurchasedProduct", "Edit Product", purchasedProduct);
        Console.WriteLine($"Product: { purchasedProduct.ProductName}");
    }

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
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    

    #endregion
}