using System.Text.Json;
using PrimeService.Model.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Account = PrimeService.Model.Common.Account;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class LicenseDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = "License Details";
    [Parameter] public Account _account { get; set; } //This comes from 'Dialog' invoker.
    [Parameter] public User _loginUser { get; set; } 
    MudForm form;
    private Account _inputMode;
    private bool _isReadOnly = false;
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    private UserAction _userAction;
    #endregion

    #region Component Initialization

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        DummyData(); //Add Dummy Data by Default. to avoid error.
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        //Dialog box opened in "Add" mode
        if (_loginUser != null)
        {
            //user.AccountId
            Utilities.ConsoleMessage($"User Account Id : {_loginUser.AccountId}");

            string url = $"{_appSettings.App.AuthUrl}{_appSettings.API.LicenseSubscriptionApi.GetAccount}";
            url = string.Format(url, _loginUser.AccountId);

            var responseModel = await _httpService.Get<Account>(url);
            if (responseModel == null)
            {
                Utilities.ConsoleMessage($"Account - Ajax Response Empty");
            }
            else
            {
                Utilities.ConsoleMessage("Account - Ajax Response Success");
                _inputMode = responseModel;
            }
        }
        
        _title = "License Subscription";
        _userAction = UserAction.ADD;
        Utilities.ConsoleMessage($"Mode : {_userAction}");
        _loading = false;
        StateHasChanged();
    }

    private void DummyData()
    {
        _inputMode = new Account()
        {
            BusinessName = "FC Prime Service",
            Subscription = new SubscriptionPlan()
            {
                PlanName = "Test Plan Subscription",
                Services = new List<SubscribedService>()
                {
                    new SubscribedService()
                    {
                        ServiceName = "Ticket",
                        CostSuffix = "/ticket",
                        QuantityLimit = 0,
                        CostPerQuantity = 0.005
                    },
                    new SubscribedService()
                    {
                        ServiceName = "Sales",
                        CostSuffix = "/bills",
                        QuantityLimit = 0,
                        CostPerQuantity = 0.05
                    },
                    new SubscribedService()
                    {
                        ServiceName = "Customer",
                        CostSuffix = "/data",
                        QuantityLimit = 0,
                        CostPerQuantity = 0.00
                    }
                }
            }
        }; //Initializes an empty object.
    }
    #endregion
    
    #region Grid View
    private MudTable<SubscribedService> table;
    private int totalItems;
    private IEnumerable<SubscribedService> pagedData;
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<SubscribedService>> ServerReload(TableState state)
    {
        Utilities.ConsoleMessage($"Rows/Page : {table.RowsPerPage}");
        var data = _inputMode.Subscription.Services;
        totalItems = data.Count();
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<SubscribedService>() {TotalItems = totalItems, Items = pagedData};
    }
    
    #endregion

}