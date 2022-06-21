using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class StatusList
{
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Status> pagedData;
    private MudTable<Status> table;
    private int totalItems;
    private string searchString = null;
    private IEnumerable<Status> _data = new List<Status>();
   
    private DialogOptions _dialogOptions = new ()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Status>> ServerReload(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketStatusApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchString)) ? string.Empty : searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Name" : state.SortLabel,
            SearchField = "Name",
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };

        IEnumerable<Status> data;
        var responseModel = await _httpService.POST<ResponseData<Status>>(url, pageMetaData);
        totalItems = responseModel.TotalItems;
        data = responseModel.Items;
        // Inline sorting. May be it is not required.
        switch (state.SortLabel)
        {
            case "Name":
                data = data.OrderByDirection(state.SortDirection, o => o.Name);
                break;
            case "Order":
                data = data.OrderByDirection(state.SortDirection, o => o.Order);
                break;
        }
        pagedData = data;
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Status>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(Status status)
    {
        Utilities.ConsoleMessage(status.Name);
        await InvokeDialog("_status","Status", status);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("_status","Status", null);
    }

    private async Task InvokeDialog(string parameter, string title, Status status)
    {
        var parameters = new DialogParameters
            { [parameter] = status }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<StatusDialog>(title, parameters, _dialogOptions);
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