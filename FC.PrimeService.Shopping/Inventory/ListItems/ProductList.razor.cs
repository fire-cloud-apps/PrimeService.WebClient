using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings.Forms;
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
    private IEnumerable<Model.Product> pagedData;
    private MudTable<Model.Product> table;
    private int totalItems;
    private string searchString = null;

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
    
    private TableGroupDefinition<Model.Product> _groupDefinition = new()
    {
        GroupName = "Category",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.Category.CategoryName
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
    private async Task<TableData<Model.Product>> ServerReload(TableState state)
    {
        IEnumerable<Model.Product> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Name":
                data = data.OrderByDirection(state.SortDirection, o => o.Name);
                break;
            case "Quantity":
                data = data.OrderByDirection(state.SortDirection, o => o.Quantity);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.Name);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Product>() {TotalItems = totalItems, Items = pagedData};
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
        _navigationManager.NavigateTo("/Action/?Component=Product");
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