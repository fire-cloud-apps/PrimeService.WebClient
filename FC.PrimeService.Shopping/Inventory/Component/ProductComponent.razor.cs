using System.Text.Json;
using FC.PrimeService.Shopping.Inventory.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Inventory.Component;

public partial class ProductComponent
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private string _display = "d-none";//Display 'none' during loading.
    public Model.Product _inputMode;
   
    /// <summary>
    /// Client unique Id to get load the data.
    /// </summary>
    [Parameter] public string? Id { get; set; } 
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    public List<Model.ProductCategory> _productCategory = new List<Model.ProductCategory>()
    {
        new Model.ProductCategory() { CategoryName = "Mobile" },
        new Model.ProductCategory() { CategoryName = "Computer" },
        new Model.ProductCategory() { CategoryName = "Laptop" },
        new Model.ProductCategory() { CategoryName = "TV" },
        
    };

    private IList<Model.ProductTransaction> _productTransactions = new List<Model.ProductTransaction>()
    {
        new Model.ProductTransaction()
        {
            TransactionDate = DateTime.Now,
            Reason = "Stock In",
            Action = Model.StockAction.Out,
            Quantity = -5,
            Price = 5 * 2500,
            Who = new Employee()
            {
                User = new User()
                {
                    Name = "SRG"
                }
            },
        },
        new Model.ProductTransaction()
        {
            TransactionDate = DateTime.Now,
            Reason = "Purchase Order",
            Action = Model.StockAction.In,
            Quantity = 15,
            Price = 15 * 2500,
            Who = new Employee()
            {
                User = new User()
                {
                    Name = "Ram"
                }
            },
        }
    };
    
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        //for now it is filled as Static value
        _inputMode = new Model.Product ()
        {
            Id = "6270d1cce5452d9169e86c50",
            Name = "Samsung M31",
            Barcode = "SKI78596KLL",
            Category = new Model.ProductCategory()
            {
                CategoryName = "Mobile"
            },
            Cost = 200,
            Quantity = 5,
            SellingPrice = 17800,
            SupplierPrice = 14000,
            Warranty = 730,
            Notes = "Samsung M31 Released 2021 Latest mobile phone",
        };
        Console.WriteLine($"Id Received: {Id}"); //Use this id and get the values from 'API'
        if (Id == null)
        {
            Console.WriteLine("Add Mode");
            _display = "d-flex";
        }
        else
        {
            Console.WriteLine("Edit Mode");
            _display = "d-none";
            //Do ajax call and assign it to _inputMode
        }
        _loading = false;
        StateHasChanged();
    }
    
    #region ServiceCategory Search - Autocomplete

    private async Task<IEnumerable<Model.ProductCategory>> ProductCategory_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _productCategory;
        }
        return _productCategory.Where(x => x.CategoryName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion
    
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
    private async Task AddStock(MouseEventArgs arg)
    {
        await InvokeDialog("_Product","Product", _inputMode, ActionType.AddStock);
    }

    private async Task ReduceStock(MouseEventArgs arg)
    {
        await InvokeDialog("_Product","Product", _inputMode, ActionType.ReduceStock);
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
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
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
    }

    enum ActionType
    {
        AddStock,
        ReduceStock
    }    

    #endregion

    private async Task LoadMore()
    {
        _navigationManager.NavigateTo($"/Inventory?viewId=PT&Id={_inputMode.Id}");
    }
}