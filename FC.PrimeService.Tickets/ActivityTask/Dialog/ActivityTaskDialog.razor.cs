using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Tickets.ActivityTask.Dialog;

public partial class ActivityTaskDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    #region Dialog Parameters
    [Parameter] public ActivityTasks TaskDetails { get; set; } 
    [Parameter] public string Title { get; set; }
    [Parameter] public UserAction UserAction { get; set; }
    [Parameter] public IList<ActivityTasks> TaskList { get; set; }
    #endregion
    private bool _processing = false;
    MudForm form;
    private ActivityTasks _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    private ActivityTasks TaskItem = null;
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    #endregion

    #region Load Async

    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager,
            _localStore, _configuration, Snackbar);
        //Dialog box opened in "Edit" mode
        _inputMode = TaskDetails;
        _title = "Enter Task";
    }

    #endregion
    
    #region Staff/Employee Search - Autocomplete

    public List<AuditUser> _employees = new List<AuditUser>();

    private async Task<IEnumerable<AuditUser>> Employee_SearchAsync(string value)
    {
        var responseData = await Utilities.GetEmployee(_appSettings, _httpService, value);
        var employees = responseData.Items;
        List<AuditUser> auditUserList = new List<AuditUser>();
        foreach (var emp in employees)
        {
            auditUserList.Add(emp.ToAuditUser(emp));
        }
        Utilities.ConsoleMessage($"Find Audit Users : '{value}'" );
        return auditUserList;
    }

    #endregion
    
    #region Cancel & Close
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    #endregion

    #region Submit Button with Animation
    

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // //Todo some animation.
            //await ProcessSomething();
            TaskList.Add(_inputMode);
            //Do server actions.
            _outputJson = JsonSerializer.Serialize(_inputMode);
            
            //Success Message
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            //Snackbar.Configuration.VisibleStateDuration  = 2000;
            //Can also be done as global configuration. Ref:
            //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
            Snackbar.Add("Task Added!", Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine(_outputJson);
            MudDialog.Close(DialogResult.Cancel());
        }
    }

    #endregion

    #region Generate Fake
    private async Task GetFakeData()
    {
        throw new NotImplementedException();
    } 
    #endregion
    
    #region PaymentTags Search - Autocomplete
    
    public List<PaymentTags> _paymentTags = new List<PaymentTags>()
    {
        new PaymentTags(){ Title = "Primary Account"},
        new PaymentTags(){ Title = "Secondary Account"},
        new PaymentTags(){ Title = "Expense Account"},
        new PaymentTags(){ Title = "Income Account"},
    };

    private async Task<IEnumerable<PaymentTags>> PaymentAccount_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);
        Console.WriteLine($"Find Account : '{value}'");
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _paymentTags;
        }

        var result = _paymentTags.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        if (result.FirstOrDefault() == null)
        {
            result = new List<PaymentTags>()
            {
                new PaymentTags() { Title = "Not Found"}
            };
        }

        return result;
    }

    #endregion
    
    #region PaymentTags Search - Autocomplete
    
    public List<PaymentMethods> _paymentMethods = new List<PaymentMethods>()
    {
        new PaymentMethods(){ Title = "Card"},
        new PaymentMethods(){ Title = "Cash"},
        new PaymentMethods(){ Title = "QR"},
    };

    private async Task<IEnumerable<PaymentMethods>> PaymentMethods_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);
        Console.WriteLine($"Find Method : '{value}'");
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _paymentMethods;
        }

        var result = _paymentMethods.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        if (result.FirstOrDefault() == null)
        {
            result = new List<PaymentMethods>()
            {
                new PaymentMethods() { Title = "Not Found"}
            };
        }

        return result;
    }

    #endregion
}