using System.Text.Json;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
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
    private DateRange _dateRange = new DateRange(DateTime.Now.AddDays(-10).Date, DateTime.Now.Date);
    
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
            AssignedTo = new AuditUser()
            {
                Name = "SRG"
            },
            EnteredBy = new AuditUser()
            {
                Name = "SR Ganesh Ram"
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
        Selector = (e) => e.TargetDate.Humanize()
    };
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    private User _loginUser;
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