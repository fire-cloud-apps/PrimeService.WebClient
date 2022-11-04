using System.Text.Json;
using FC.PrimeService.Dashboards.Dashboard.CardPanel;
using FC.PrimeService.Payments.Payment.Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
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
    private int totalItems;
    [Parameter] public string? Id { get; set; } 
    private string searchString = null;
    DateRange _dateRange = new DateRange(DateTime.Now.AddDays(-7), DateTime.Now.Date);
    private AuditUser _loginUser = null;
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    
    private DateRange SelectedDateRange
    {
        get
        {
            return _dateRange;
        }
        set
        {
            _dateRange = value;
            string dtRange = $"{_dateRange.Start}-{_dateRange.End}";
            Utilities.ConsoleMessage($"Date Range - {dtRange}");
            OnSearch(dtRange, "Range");
        }
    }

    #region Category Selection

    private MudListItem _selectedCategory;
    private MudListItem SelectedCategory
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
                if (_selectedCategory.Value.ToString() == "-1")
                {
                    OnSearch(string.Empty, "PaymentCategory");
                }
                else
                {
                    OnSearch(_selectedCategory.Text, "PaymentCategory");
                }
                Utilities.ConsoleMessage($"Selected Item Payment Category: {_selectedCategory.Value} - Text: { _selectedCategory.Text}");
            }
        }
    }

    
    #endregion


    private IEnumerable<Model.Payments> _data = new List<Model.Payments>();
   
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.Payments> _groupDefinition = new()
    {
        GroupName = "Staff",
        Indentation = true,
        Expandable = false,
        Selector = (e) => e.Who.Name
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call Initialized.
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion

        _loginUser = await Utilities.GetLoginUser(_localStore);
        
        _loading = false;
        StateHasChanged();
    }
    #endregion
    
    #region Grid View
    private PaymentCard _payCard;
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.Payments> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = string.Empty;
    private string _searchField = "Name";
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.Payments>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        //Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Payments>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }
    
    /// <summary>
    /// Do Ajax call to get 'Payments' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>Payments Data.</returns>
    private async Task<ResponseData<Model.Payments>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Reason" : state.SortLabel,
            SearchField = _searchField,
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.Payments>>(url, pageMetaData);
        return responseModel;
    }
    
    private void OnSearch(string text, string field = "Reason")
    {
        _payCard.ReloadCardValue(SelectedDateRange);
        //_payCard.FilterDateRange
        
        _searchString = text;
        _searchField = field;
        _mudTable.ReloadServerData();//If we put Async, Loading progress bar is not closing.
        StateHasChanged();
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
    
    #region Invoke Payment Dialog

    private async Task AddIncome()
    {
        await InvokeDialog("Add Income",UserAction.ADD, new Model.Payments()
        {
            PaymentCategory = PaymentCategory.Income,
            Who = _loginUser
        });//Null indicates its an 'Add' Mode.
    }
    private async Task AddExpense()
    {
        await InvokeDialog("Add Expense",UserAction.ADD, new Model.Payments()
        {
            PaymentCategory = PaymentCategory.Expense,
            Who = _loginUser,
        });//Null indicates its an 'Add' Mode.
    }
    
    private async Task InvokeDialog(string title, 
        UserAction action = UserAction.ADD, Model.Payments model = null)
    {
        var parameters = new DialogParameters
        {
            ["Payments"] = model,
            ["UserAction"] =  action as object,
            ["Title"] = title
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<PaymentDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;
        
        if (result.Cancelled)
        {
            Utilities.ConsoleMessage("Cancelled.");
            OnSearch(string.Empty);
        }
        else
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            Utilities.ConsoleMessage("Executed.");
            OnSearch(string.Empty);//Reload the server grid.
        }
    }

    #endregion
}