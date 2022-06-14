using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;

namespace FC.PrimeService.Tickets.Ticket.ListItems;

public  partial class TicketList
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
    private IEnumerable<Model.TicketService> pagedData;
    private MudTable<Model.TicketService> table;
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
    
    
    IEnumerable<Model.TicketService> _data = new List<Model.TicketService>()
    {
        new Model.TicketService()
        {
            Id = "343093409jdl934l34l43",
            TicketNo = "TN#5.27.2022.1",
            CreatedDate = DateTime.Now,
            TicketStatus = new Status(){ Name = "InProgress" },
            AdvanceAmount = 2000,
            Appearance = "Slightly Damaged",
            BalanceAmount = 3000,
            TargetDate = DateTime.Now.AddDays(2),
            TotalAmount = 5000,
            Reasons = "Audio is not clear",
            AssignedTo =  new Employee()
            {
                    User = new User()
                    {
                    Name = "Alam"
                }
            },
            EnteredBy = new Employee()
            {
                User = new User()
                {
                    Name = "SR Ganesh Ram"
                }
            },
            Client = new Shop.Client()
            {
                Name = "SRG",
                Mobile = "8589645123"
            }
        
        },
        new Model.TicketService()
        {
            Id = "85893409jdl934l34l43",
            TicketNo = "TN#5.30.2022.1",
            CreatedDate = DateTime.Now.AddDays(5),
            TicketStatus = new Status(){ Name = "Init" },
            AdvanceAmount = 5000,
            Appearance = "Slightly Damaged",
            BalanceAmount = 5000,
            TargetDate = DateTime.Now.AddDays(5),
            TotalAmount = 10000,
            Reasons = "Audio is not clear",
            AssignedTo =  new Employee()
            {
                User = new User()
                {
                    Name = "Alam"
                }
            },
            EnteredBy = new Employee()
            {
                User = new User()
                {
                    Name = "SR Ganesh Ram"
                }
            },
            Client = new Shop.Client()
            {
                Name = "SRG",
                Mobile = "8589645123"
            }
        
        },
        new Model.TicketService()
        {
            Id = "58899409OLKI934l34l43",
            TicketNo = "TN#5.31.2022.3",
            CreatedDate = DateTime.Now.AddDays(5),
            TicketStatus = new Status(){ Name = "Init" },
            AdvanceAmount = 5000,
            Appearance = "No Physical Damage found",
            BalanceAmount = 5000,
            TargetDate = DateTime.Now.AddDays(5),
            TotalAmount = 10000,
            Reasons = "Audio is not clear",
            AssignedTo =  new Employee()
            {
                User = new User()
                {
                    Name = "SRG"
                }
            },
            EnteredBy = new Employee()
            {
                User = new User()
                {
                    Name = "SR Ganesh Ram"
                }
            },
            Client = new Shop.Client()
            {
                Name = "SRG",
                Mobile = "8589645123"
            }
        
        }
    };

    #region Ticket Status Filter

    private List<Status> _ticketStatus = new List<Status>
    {
        new Status()
        {
            Name = "Init",
            ColorCode = "#2196f3",//Blue
        },
        new Status()
        {
            Name = "In Progress",
            ColorCode = "#0989c2"
            
        },
        new Status()
        {
            Name = "Waiting",
            ColorCode = "#ffe273"
        },
        new Status()
        {
            Name = "Hold",
            ColorCode = "#d9980d"
        },
        new Status()
        {
            Name = "Completed",
            ColorCode = "#00c853"
        },
        new Status()
        {
            Name = "Closed",
            ColorCode = "#353940"
        },
    };

    #endregion
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.TicketService> _groupDefinition = new()
    {
        GroupName = "Target Date",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.TargetDate
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
    private async Task<TableData<Model.TicketService>> ServerReload(TableState state)
    {
        IEnumerable<Model.TicketService> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.TicketNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Date":
                data = data.OrderByDirection(state.SortDirection, o => o.CreatedDate);
                break;
            case "TicketStatus":
                data = data.OrderByDirection(state.SortDirection, o => o.TicketStatus.Name);
                break;
            case "Balance":
                data = data.OrderByDirection(state.SortDirection, o => o.BalanceAmount);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.TargetDate);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.TicketService>() {TotalItems = totalItems, Items = pagedData};
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
        var url = $"/Ticket?viewId=Ticket";
        //Navigate and open in new tab.
        await  JSRuntime.InvokeAsync<object>("open", 
            new object[2] { url, "_blank" });
    }
    #endregion

    private async Task AddProductCategory()
    {
        await InvokeDialog("_ProductCategory","Product Category", null);//Null indicates its an 'Add' Mode.
    }
    private async Task InvokeDialog(string parameter, string title, Model.TicketService model)
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