using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Inventory.ListItems;

public partial class ProductTransactionList
{
    #region Variables
    [Inject] 
    ISnackbar Snackbar { get; set; }
    /// <summary>
    /// Product Id
    /// </summary>
    [Parameter]
    public string Id { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Model.ProductTransaction> pagedData;
    private MudTable<Model.ProductTransaction> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<Model.ProductTransaction> _data = new List<Model.ProductTransaction>()
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
        },
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
    },
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
    },
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
    },
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
    },
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
    },
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
    },
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
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.ProductTransaction> _groupDefinition = new()
    {
        GroupName = "Stock",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.Action
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        if (string.IsNullOrEmpty(Id))
        {
            //Add Mode
        }
        else
        {
            //This case always we should get the _Id.
            Console.WriteLine($"Product Id : {Id}");
            //Edit Mode.
        }
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<Model.ProductTransaction>> ServerReload(TableState state)
    {
        IEnumerable<Model.ProductTransaction> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Who.User.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Quantity":
                data = data.OrderByDirection(state.SortDirection, o => o.Quantity);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.Price);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.ProductTransaction>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Print the Transactions
    private async Task PrintIt(MouseEventArgs arg)
    {
        //to do some printing activity.
        //We can refer: https://github.com/Append-IT/Blazor.Printing
        //_navigationManager.NavigateTo("/Action/?Component=Product");
    }
    #endregion
}