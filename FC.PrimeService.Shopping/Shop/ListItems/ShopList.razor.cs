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
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Shop.ListItems;

public partial class ShopList
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
    private IEnumerable<Model.Sales> pagedData;
    private MudTable<Model.Sales> table;
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
        }
    }

    #region Category Selection

    private MudListItem _selectedCategory;
    public MudListItem SelectedCategory
    {
        get
        {
            return _selectedCategory;
        }
        set
        {
            _selectedCategory = value;
            if (_selectedCategory != null)
            {
                Console.WriteLine($"Selected Item Category {_selectedCategory.Text}");
            }
        }
    }

    
    #endregion
    
    
    IEnumerable<Model.Sales> _data = new List<Model.Sales>()
    {
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.1",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Closed,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
           TotalDiscount = 50,
           SubTotal = 501,
           TotalTax = 52,
           PaymentStatus = Model.PaymentStatus.Paid,
           TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.4",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.OnHold,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Pending,
            TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.1",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Void,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
           TotalDiscount = 50,
           SubTotal = 501,
           TotalTax = 52,
           PaymentStatus = Model.PaymentStatus.Paid,
           TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.4",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.OnHold,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Pending,
            TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.1",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Confirmed,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
           TotalDiscount = 50,
           SubTotal = 501,
           TotalTax = 52,
           PaymentStatus = Model.PaymentStatus.Paid,
           TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.4",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.OnHold,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Pending,
            TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.1",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Confirmed,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
           TotalDiscount = 50,
           SubTotal = 501,
           TotalTax = 52,
           PaymentStatus = Model.PaymentStatus.Paid,
           TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.4",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.OnHold,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Pending,
            TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.1",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Confirmed,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
           TotalDiscount = 50,
           SubTotal = 501,
           TotalTax = 52,
           PaymentStatus = Model.PaymentStatus.Paid,
           TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.4",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.OnHold,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Pending,
            TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.1",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Confirmed,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
           TotalDiscount = 50,
           SubTotal = 501,
           TotalTax = 52,
           PaymentStatus = Model.PaymentStatus.Paid,
           TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.4",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.OnHold,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Pending,
            TransactionDate = DateTime.Now
        },
        new Model.Sales()
        {
            BillNumber = "#2022.05.03.2",
            BilledBy = new AuditUser(){  Name = "SRG"},
            Client = new Model.Client(){ Name = "Alam"},
            AdditonalCost = 30,
            Id = "626f61717c3a4477dc2d8275",
            Notes = "Samsung M31, Samsung M33, Lenovo",
            GrandTotal = 1230.02d,
            TotalQuantity = 3,
            Products =  new List<Model.PurchasedProduct>()
            {
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 256,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 250,
                    ProductId = "626f61717c3a4477dc2d8275",
                    ProductName = "Samsung M31",
                    SubTotal = 250

                },
                new Model.PurchasedProduct() {
                    Discount = 3,
                    Price = 366,
                    Quantity = 1,
                    AppliedTax = new Tax(){  Title = "Elecronic Tax", TaxRate = 5.8f},
                    DiscountPrice = 16,
                    ProductId = "626f61717c3a4477dc2d8285",
                    ProductName = "Samsung M33",
                    SubTotal = 350

                },
                
            },
            Status = Model.SalesStatus.Confirmed,
            PaymentAccount = new PaymentTags(){ Title = "Primary Account"},
            PaymentMethod = new PaymentMethods(){ Title = "Card"},
            TotalDiscount = 50,
            SubTotal = 501,
            TotalTax = 52,
            PaymentStatus = Model.PaymentStatus.Paid,
            TransactionDate = DateTime.Now
        }
        
    };
    
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
        Expandable = false,
        Selector = (e) => e.Status
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<Model.Sales>> ServerReload(TableState state)
    {
        IEnumerable<Model.Sales> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Notes.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Date":
                data = data.OrderByDirection(state.SortDirection, o => o.TransactionDate);
                break;
            case "SalesStatus":
                data = data.OrderByDirection(state.SortDirection, o => o.Status);
                break;
            case "Quantity":
                data = data.OrderByDirection(state.SortDirection, o => o.TotalQuantity);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.GrandTotal);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Sales>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
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
}