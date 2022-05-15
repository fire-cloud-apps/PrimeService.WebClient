using System.Text.Json;
using FC.PrimeService.Shopping.Inventory.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Win32.SafeHandles;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
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
        _inputMode = new Model.Sales ()
        {
            Id = "6270d1cce5452d9169e86c50",
            Client = new Model.Client()
            {
                Name = "Guest", Mobile = "1234567890"
            }
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
    private async Task AddStock(MouseEventArgs arg)
    {
        //await InvokeDialog("_Product","Product", _inputMode, ActionType.AddStock);
    }

    private async Task ReduceStock(MouseEventArgs arg)
    {
        //await InvokeDialog("_Product","Product", _inputMode, ActionType.ReduceStock);
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
    
    #region Product Search - Autocomplete

    private Model.Product _selectedProduct = new Model.Product();
    public List<Model.Product> _productList = new List<Model.Product>()
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
        return _productList.Where(x => x.Barcode.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task ProductSelected_Changed(Model.Product selectedProduct)
    {
        if (selectedProduct != null)
        {
            Console.WriteLine($"Selected Product - Name: {selectedProduct.Name} | BarCode: {selectedProduct.Barcode}");
        }
        
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


    
}