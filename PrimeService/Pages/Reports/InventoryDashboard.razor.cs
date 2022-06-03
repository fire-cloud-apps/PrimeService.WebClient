using Microsoft.JSInterop;
using MudBlazor;

namespace FireCloud.WebClient.PrimeService.Pages.Reports;

public partial class InventoryDashboard
{
       #region Date Range Selection
    private bool _loading = false;
    private string _rangeText = string.Empty;
    private int _defaultDateRange = 7;
    private DateRange _dateRange = new DateRange(DateTime.Now.AddDays(-7), DateTime.Now);
    private DateRange SelectedDateRange
    {
        get { return _dateRange; }
        set
        {
            _dateRange = value;
            Console.WriteLine($"Selected Date Range - From :{_dateRange.Start} End : {_dateRange.End}");
        }
    }

    #endregion
    
    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await Task.Delay(2000);
        _dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.AddDays(_defaultDateRange).Date);
        _rangeText = $"Trend Report for the past {_defaultDateRange} Days";
        //await InvokeTrendChart();
        _loading = false;
        //An Ajax call to get company details
        StateHasChanged();
    }

    private int _renderCount = 0;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _renderCount++;
        if (firstRender)
        {
            var targetUrl = "js/trendsChart.js";
            await jsRuntime.InvokeVoidAsync("loadJs", targetUrl);
            await Task.Delay(1000);
            Console.WriteLine("Trend Chart Script loaded");
        }
        else
        {
            if (_renderCount == 2)
            {
                await TicketByDate_Chart();
                //await TicketByStatus_Chart();
                await SalesByDate_Chart();
                Console.WriteLine("Chart Invoked");
            }
        }
    }
    #endregion

    #region Charts
    private async Task TicketByDate_Chart()
    {
        //await jsRuntime.InvokeAsync<bool>("InvokeTicketChart", null);
        await jsRuntime.InvokeAsync<bool>("ticketByDate",
            new object[] {
                SelectedDateRange.Start.Value.Date, SelectedDateRange.End.Value.Date
            }
        );
    }
    private async Task SalesByDate_Chart()
    {
        await jsRuntime.InvokeAsync<bool>("salesByStatus",
            new object[] {
                SelectedDateRange.Start.Value.Date, SelectedDateRange.End.Value.Date
            }
        );
    }
    private async Task RefreshData()
    {
        await TicketByDate_Chart();
    }
    #endregion 
}