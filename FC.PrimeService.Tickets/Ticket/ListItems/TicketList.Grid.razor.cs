using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;

namespace FC.PrimeService.Tickets.Ticket.ListItems;

public partial class TicketList
{
    
    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.TicketService>? _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = string.Empty;
    private string _searchField = "Name";
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.TicketService>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.TicketService>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }
    
    /// <summary>
    /// Do Ajax call to get 'TicketService' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>Sales Data.</returns>
    private async Task<ResponseData<Model.TicketService>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Name" : state.SortLabel,
            SearchField = _searchField,
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.TicketService>>(url, pageMetaData);
        return responseModel;
    }

    private void OnSearch(string text, string field = "TicketNo")
    {
        _searchString = text;
        _searchField = field;
        _mudTable?.ReloadServerData();//If we put Async, Loading progress bar is not closing.
        StateHasChanged();
    }
    #endregion
    
}