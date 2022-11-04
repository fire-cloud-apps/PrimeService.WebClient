using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Dashboards;


namespace FC.PrimeService.Dashboards.Dashboard.CardPanel;

public partial class PaymentCard
{
    #region Initialization
    [Inject] 
    ISnackbar Snackbar { get; set; }
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    private Model.PaymentCard _inputMode;

    [Parameter]
    public DateRange? FilterDateRange { get; set; } = new DateRange(DateTime.Now.AddDays(-7), DateTime.Now.Date);
    #endregion
    
    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        #region Ajax Call Initialized.
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion

        _inputMode = new Model.PaymentCard();
        await ReloadCardValue(FilterDateRange);
        StateHasChanged();
    }
    #endregion

    #region Over all Account Balance

    private bool _loading = false;
    private string _accountBalance = "0";
    public async Task ReloadCardValue(DateRange dateRange)
    {
        _loading = true;
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.PaymentApi.PayCard}";
        string dtRange = $"{dateRange.Start}-{dateRange.End}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = dtRange,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = "Range.PaymentCard",
            SortDirection = "A"
        };
        
        var responseModel = await _httpService.POST<ResponseData<Model.PaymentCard>>(url, pageMetaData);
        
        _inputMode = responseModel.Items.FirstOrDefault();
        _loading = false;
        StateHasChanged();
    }

    #endregion
}