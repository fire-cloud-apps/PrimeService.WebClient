using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Utility;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;
using Pay = PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Tickets.Ticket.Component;

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
    public IEnumerable<ClientType> _clientTypes = new List<ClientType>();

    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    private string _subTitle = string.Empty;
    private AuditUser _loginUser = null;
    private UserAction _userAction;
    Func<Status,string> StatusConverter = p => p?.Name;
    #endregion
    
    #region Tasks
    public bool IsCompleted { get; set; }
    private Model.ActivityTasks TaskItem { get; set; }
    #endregion

    #region ServiceType Assignment

    private ServiceType _selectedServiceType;
    private ServiceType SelectedServiceType
    {
        get
        {
            return _selectedServiceType;
        }
        set
        {
            _selectedServiceType = value;
            _inputMode.ServiceType = _selectedServiceType;
            CalculateAmount();
        }
    }

    #endregion
        
    #region Payment Lists

    private IList<Pay.Payments> _paymentDetails = new List<Pay.Payments>();
    
    #endregion
    
    #region On Initialization Async
    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        _loading = true;
        _httpService = new HttpService(_httpClient, _navigationManager, 
            _localStore, _configuration, Snackbar);
        _loginUser = await Utilities.GetLoginUser(_localStore);
        var clientType = await Utilities.GetClientType(_appSettings, _httpService);
        _clientTypes = clientType.Items;
        var status = await Utilities.GetTicketStatus(_appSettings, _httpService);
        _ticketStatus = status.Items;
        if (string.IsNullOrEmpty(Id))
        {
            _userAction = UserAction.ADD;
            await InitializeAddMode();
        }
        else
        {
            _userAction = UserAction.EDIT;
            await InitializeEditMode();
        }
        StateHasChanged();
        _loading = false;
    }

    #region Add/Edit Mode Initialized

    private async Task InitializeAddMode()
    {
        _userAction = UserAction.ADD;
        
        TaskItem = new Model.ActivityTasks()
        {
            Title = string.Empty,
            IsCompleted = false,
            TargetDate = DateTime.Now,
            AssignedTo = new AuditUser() { Name = "SRG" },
            Notes = "Some Additional Notes"
        };
        _inputMode = new Model.TicketService()
        {
            TicketNo = await GenerateTicketNo(),
            CreatedDate = DateTime.Now,
            EnteredBy = _loginUser,
            Client = new Shop.Client()
            {
                Name = "Guest",
                Mobile = "10-10000-10000",
                Type = _clientTypes.FirstOrDefault()
            },
            
            ServiceType =  new ServiceType()
            {
                Type = Model.TicketType.GeneralService
            },
            AdvanceAmount = 0,
            //Payments = _paymentDetails,
            PaymentAccount = await Utilities.GetDefault_PaymentAccount(_appSettings, _httpService),
            TargetDate = DateTime.Now.AddDays(3),
            TicketStatus = _ticketStatus.FirstOrDefault(),
            Appearance = "Normal",
            Reasons = "Service",
            
            TicketTypeDetails = Model.TicketProperty.GetTicketCustomProperty(Model.TicketType.SmartPhone),
            AdditionalDetails = new List<CustomField>(),
            //Activities = _activityLogs, //Should be assigned at the time of 'Save'
            BillNumbers = new List<string>(),
            //UserLastComments = "Last Comments"
        };
        _subTitle = _inputMode.CreatedDate.ToString("dddd, dd MMMM yyyy")
                    + " by " + _inputMode.EnteredBy.Name;
        Console.WriteLine($"Id Received: {Id}"); //Use this id and get the values from 'API'
        _loading = false;
        //SelectedStatus = _ticketStatus.FirstOrDefault();
        _inputMode.TicketStatus = _ticketStatus.FirstOrDefault();
        CalculateAmount();
    }

    private async Task InitializeEditMode()
    {
        //1. Get the Ticket Service data fully & Bind.
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.GetDetails}";
        url = string.Format(url, Id);
        Utilities.ConsoleMessage($"Get Ticket Details URL : {url}");
        _inputMode = await _httpService.GET<Model.TicketService>(url);
        _subTitle = _inputMode.CreatedDate.ToString("dddd, dd MMMM yyyy")
                    + " by " + _inputMode.EnteredBy.Name;

        SelectedServiceType = _inputMode.ServiceType;
        //SelectedStatus = _inputMode.TicketStatus;
        CalculateAmount();
    }

    #endregion
    #endregion
    
    #region Ticket Status
    
    private IEnumerable<Status> _ticketStatus = new List<Status>();
    
    #endregion
    
    #region Generate Sales Unique Id
    private async Task<string> GenerateTicketNo()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.GenerateTicketNo}";
        Utilities.ConsoleMessage($"Generate Bill No: {url}");
        var serialNumber = await _httpService.GET<SerialNumber>(url);
        _loading = false;
        return serialNumber.Preview;
    }
    #endregion

    private async Task CalculateAmount_Changed(FocusEventArgs arg)
    {
        CalculateAmount();
    }

    #region Payment Items Remove

    MudMessageBox mbox { get; set; }
    private string state;
    private async Task RemovePaymentItem(Pay.Payments  paymentItem)
    {
        bool? result = await mbox.Show();
        state = result == null ? "Cancelled" : "Deleted!";
        int? itemIndex = 0;
        if (state == "Deleted!")
        {
            foreach (Pay.Payments pay in _inputMode.Payments)
            {
                if (pay.Id == paymentItem.Id)
                {
                    itemIndex =  _inputMode.Payments.IndexOf(pay);
                    break;
                }
            }
            _inputMode.Payments.RemoveAt(itemIndex.Value);
            CalculateAmount();
            Snackbar.Add("Item Removed", Severity.Error);
        }
        StateHasChanged();
    }

    #endregion
}