using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Tickets;
using Model = PrimeService.Model.Settings.Payments;
using Shop = PrimeService.Model.Shopping;

namespace FC.PrimeService.Payments.Payment.ListItems;

public partial class PaymentList
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
    private IEnumerable<Model.Payments> pagedData;
    private MudTable<Model.Payments> table;
    private int totalItems;
    [Parameter] public string? Id { get; set; } 
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
    
    
    IEnumerable<Model.Payments> _data = new List<Model.Payments>()
    {
        new Model.Payments()
        {
            Id = "343093409jdl934l34l43",
            TransactionDate = DateTime.Now,
            PaymentCategory = PaymentCategory.Expense,
            Reason = "Purchase product",
            Who = new Employee(){ User = new User(){ Name = "SRG"}},
            ExpenseAmount = 2500,
            IncomeAmount = 0,
            PaymentMethod = new PaymentMethods(){Title = "Card"},
            PaymentTag = new PaymentTags(){ Title = "Expense Account"}
        },
        new Model.Payments()
        {
            Id = "385893409jdl934l34l43",
            TransactionDate = DateTime.Now.AddDays(2),
            PaymentCategory = PaymentCategory.Expense,
            Reason = "Product product",
            Who = new Employee(){ User = new User(){ Name = "SRG"}},
            ExpenseAmount = 5000,
            IncomeAmount = 0,
            PaymentMethod = new PaymentMethods(){Title = "Cash"},
            PaymentTag = new PaymentTags(){ Title = "Expense Account"}
        },
        new Model.Payments()
        {
            Id = "855893409jdl934l34l43",
            TransactionDate = DateTime.Now.AddDays(2),
            PaymentCategory = PaymentCategory.Expense,
            Reason = "Sales Product",
            Who = new Employee(){ User = new User(){ Name = "SRG"}},
            ExpenseAmount = 6000,
            IncomeAmount = 0,
            PaymentMethod = new PaymentMethods(){Title = "Card"},
            PaymentTag = new PaymentTags(){ Title = "Expense Account"}
        },
        new Model.Payments()
        {
            Id = "LOLO940409jdl934l34l43",
            TransactionDate = DateTime.Now.AddDays(5),
            PaymentCategory = PaymentCategory.Income,
            Reason = "Ticket Service",
            Who = new Employee(){ User = new User(){ Name = "Alam"}},
            ExpenseAmount = 10000,
            IncomeAmount = 0,
            PaymentMethod = new PaymentMethods(){Title = "Card"},
            PaymentTag = new PaymentTags(){ Title = "Income Account"}
        },
    };
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.Payments> _groupDefinition = new()
    {
        GroupName = "Date",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.TransactionDate
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
    private async Task<TableData<Model.Payments>> ServerReload(TableState state)
    {
        IEnumerable<Model.Payments> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Reason.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Date":
                data = data.OrderByDirection(state.SortDirection, o => o.TransactionDate);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.TransactionDate);
                break;
        }
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Payments>() {TotalItems = totalItems, Items = pagedData};
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
        var url = $"/Payments?viewId=Payment";
        //Navigate and open in new tab.
        await  JSRuntime.InvokeAsync<object>("open", 
            new object[2] { url, "_blank" });
    }
    #endregion

    private async Task AddProductCategory()
    {
        await InvokeDialog("_ProductCategory","Product Category", null);//Null indicates its an 'Add' Mode.
    }
    private async Task InvokeDialog(string parameter, string title, Model.Payments model)
    {
        // var parameters = new DialogParameters
        //     { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        // var dialog = DialogService.Show<ProductCategoryDialog>(title, parameters, _dialogOptions);
        // var result = await dialog.Result;
        //
        // if (!result.Cancelled)
        // {
        //     Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        // }
    }
}