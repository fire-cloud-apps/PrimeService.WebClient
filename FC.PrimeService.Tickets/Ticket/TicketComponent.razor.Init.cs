using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson.Serialization.IdGenerators;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Location;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using Color = MudBlazor.Color;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;
using Settings = PrimeService.Model.Settings;

namespace FC.PrimeService.Tickets.Ticket;

public partial class TicketComponent
{
    #region Variable Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private Model.TicketService? _inputMode;
    /// <summary>
    /// To be updated in the 'ActivityLog' as user comments
    /// </summary>
    private string _userComments = string.Empty;

    /// <summary>
    /// Client unique Id to get load the data.
    /// </summary>
    [Parameter]
    public string? Id { get; set; } = string.Empty;
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = false;
    public List<ClientType> _clientTypes = new List<ClientType>()
    {
        new ClientType() { Title = "Individual" },
        new ClientType() { Title = "Company" },
        new ClientType() { Title = "NGO" },
        new ClientType() { Title = "Government" },
    };

    public string _subTitle = string.Empty;
    #endregion

    #region Tasks
    public bool IsCompleted { get; set; }
    private Model.ActivityTasks TaskItem { get; set; }
    #endregion

    #region On Initialization Async

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        _loading = true;
        await Task.Delay(500);
        TaskItem = new Model.ActivityTasks()
        {
            Title = string.Empty,
            IsCompleted = false,
            TargetDate = DateTime.Now,
            AssignedTo = new Employee(){ User = new User(){ Name = "SRG"}},
            Notes = "Some Additional Notes"
        };
        //An Ajax call to get company details
        //for now it is filled as Static value
        _inputMode = new Model.TicketService()
        {
            TicketNo = "TN#5.27.2022.1",
            CreatedDate = DateTime.Now,
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
        
        };
        _subTitle = _inputMode.CreatedDate.ToString("dddd, dd MMMM yyyy")
                    + " by " + _inputMode.EnteredBy.User.Name;
        Console.WriteLine($"Id Received: {Id}"); //Use this id and get the values from 'API'
        _loading = false;
        SelectedStatus = _ticketStatus.FirstOrDefault();
        StateHasChanged();

    }

    #endregion

    #region Staff/Employee Search - Autocomplete
    
    public List<Employee> _employees = new List<Employee>()
    {
       new Employee(){ User = new User(){ Name = "SRG"}},
       new Employee(){ User = new User(){ Name = "Alam"}},
       new Employee(){ User = new User(){ Name = "Priti"}},
       new Employee(){ User = new User(){ Name = "Joshmi"}},
    };

    private async Task<IEnumerable<Employee>> Employee_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);
        Console.WriteLine($"Find Client : '{value}'");
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _employees;
        }

        var result = _employees.Where(x => x.User.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        if (result.FirstOrDefault() == null)
        {
            result = new List<Employee>()
            {
                new Employee() { User = new User() { Name = "Not Found" } }
            };
        }

        return result;
    }

    #endregion
    
    #region Client Search - Autocomplete
    public List<Shop.Client> _clients = new List<Shop.Client>()
    {
        new Shop.Client() { Name = "SRG", Mobile = "8596963"},
        new Shop.Client() { Name = "Alam", Mobile = "96936"},
        new Shop.Client() { Name = "Pritish", Mobile = "74512"},
        new Shop.Client() { Name = "Joshmitha", Mobile = "556326"},
    };
    private async Task<IEnumerable<Shop.Client>> Client_SearchAsync(string value)
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
            result = new List<Shop.Client>()
            {
                new Shop.Client() { Name = "Customer Not found", Mobile = "" }
            };
        }
        return result;
    }

    #endregion
    
    #region Payment Lists

    private IList<Model.PaymentDetails> _paymentDetails = new List<Model.PaymentDetails>()
    {
        new Model.PaymentDetails()
        {
            PayedAmount = 100,
            PaymentAccount = new PaymentTags()
            {
                Title = "Primary Account",
            },
            PaymentMethod = new PaymentMethods()
            {
                Title = "Card"
            },
            PaymentStatus = Shop.PaymentStatus.Pending,
            ReferenceNumber = "#001",
            TransactionDate = new DateTime(2022, 05,25),
            
        },
        new Model.PaymentDetails()
        {
            PayedAmount = 300,
            PaymentAccount = new PaymentTags()
            {
                Title = "Primary Account",
            },
            PaymentMethod = new PaymentMethods()
            {
                Title = "Cash"
            },
            PaymentStatus = Shop.PaymentStatus.Pending,
            ReferenceNumber = "#002",
            TransactionDate = new DateTime(2022, 05,26)
        },
        new Model.PaymentDetails()
        {
            PayedAmount = 250,
            PaymentAccount = new PaymentTags()
            {
                Title = "Primary Account",
            },
            PaymentMethod = new PaymentMethods()
            {
                Title = "QR"
            },
            PaymentStatus = Shop.PaymentStatus.Pending,
            ReferenceNumber = "#003",
            TransactionDate = new DateTime(2022, 05,27)
        },
        new Model.PaymentDetails()
        {
            PayedAmount = 150,
            PaymentAccount = new PaymentTags()
            {
                Title = "Primary Account",
            },
            PaymentMethod = new PaymentMethods()
            {
                Title = "CardLess"
            },
            PaymentStatus = Shop.PaymentStatus.Pending,
            ReferenceNumber = "#004",
            TransactionDate = new DateTime(2022, 05,28)
        },
    };

    #endregion


    
    #region Ticket Status
    private Status SelectedStatus { get; set; }

    private List<Status> _ticketStatus = new List<Status>
    {
        new Status()
        {
            Name = "Init",
            ColorCode = StatusColor.Info
        },
        new Status()
        {
            Name = "In Progress",
            ColorCode = StatusColor.Primary
            
        },
        new Status()
        {
            Name = "Waiting",
            ColorCode = StatusColor.Secondary
        },
        new Status()
        {
            Name = "Hold",
            ColorCode = StatusColor.Warning
        },
        new Status()
        {
            Name = "Completed",
            ColorCode = StatusColor.Success
        },
        new Status()
        {
            Name = "Closed",
            ColorCode = StatusColor.Default
        },
    };

    #endregion
    
    #region Activity Lists

    private IList<Model.ActivityLog> _activityLogs = new List<Model.ActivityLog>()
    {
        new Model.ActivityLog()
        {
            ActivityDate = DateTime.Now,
            // AssignedFrom = new Employee() { User = new User() { Name = "Kumar" } },
            // AssignedTo = new Employee(){ User = new User() { Name = "SRG" } },
            // ByWho = { User = new User() { Name = "Ram" } },
            // FromStatus = new Status() {Name = "New"},
            // ToStatus = new Status() {Name = "In Progress"},
            // UserComments = "Service Initiated."
        },
        // new Model.ActivityLog()
        // {
        //     ActivityDate = DateTime.Now.AddDays(2),
        //     AssignedFrom = new Employee() { User = new User() { Name = "SRG" } },
        //     AssignedTo = new Employee(){ User = new User() { Name = "Pritish" } },
        //     ByWho = { User = new User() { Name = "Ram" } },
        //     FromStatus = new Status() {Name = "New"},
        //     ToStatus = new Status() {Name = "In Progress"},
        //     UserComments = "Issue Found, need to replace the RAM."
        // }
    };
    
    #endregion
}