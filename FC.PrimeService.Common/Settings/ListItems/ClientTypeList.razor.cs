using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class ClientTypeList
{
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private ClientType _inputMode;
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;

    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<ClientType> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = null;
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<ClientType>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<ClientType>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }

    /// <summary>
    /// Do Ajax call to get 'ClientType' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>ClientType Data.</returns>
    private async Task<ResponseData<ClientType>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ClientTypeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(_searchString)) ? string.Empty : _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Title" : state.SortLabel,
            SearchField = "Title",
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<ClientType>>(url, pageMetaData);
        return responseModel;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _mudTable.ReloadServerData();//If we put Async, Loading progress bar is not closing.
        StateHasChanged();
    }
    #endregion
    
    #region Dialog Open Action
    private DialogOptions _dialogOptions = new ()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    private async Task OpenEditDialog(ClientType model)
    {
        Utilities.ConsoleMessage(JsonSerializer.Serialize(model));
        await InvokeDialog("Edit Client Type", UserAction.EDIT, model:model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("Add Client Type",UserAction.ADD);
    }
    
    private async Task InvokeDialog(string title, 
        UserAction action = UserAction.ADD, ClientType model = null)
    {
        var parameters = new DialogParameters
        {
            ["ClientType"] = model,
            ["UserAction"] =  action as object,
            ["Title"] = title
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<ClientTypeDialog>(title, parameters, _dialogOptions);
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