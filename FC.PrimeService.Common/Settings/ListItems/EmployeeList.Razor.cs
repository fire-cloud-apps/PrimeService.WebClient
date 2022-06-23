using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class EmployeeList
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    private bool _loading = false;
    private Employee _inputMode;
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private User _loginUser;
    
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
        _loginUser = await _localStore.GetItemAsync<User>("user");
        Utilities.ConsoleMessage($"Login User {_loginUser.AccountId}");
        StateHasChanged();
    }
    #endregion
    
    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Employee> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = null;
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Employee>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Employee>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }

    /// <summary>
    /// Do Ajax call to get 'WorkLocation' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>WorkLocation Data.</returns>
    private async Task<ResponseData<Employee>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.EmployeeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(_searchString)) ? string.Empty : _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Title" : state.SortLabel,
            SearchField = "Title",
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Employee>>(url, pageMetaData);
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
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Large, 
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    private async Task OpenEditDialog(Employee model)
    {
        Utilities.ConsoleMessage(JsonSerializer.Serialize(model));
        await InvokeDialog("Edit Employee", UserAction.EDIT, model:model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("Add Employee",UserAction.ADD);
    }
    
    private async Task InvokeDialog(string title, 
        UserAction action = UserAction.ADD, Employee model = null)
    {
        var parameters = new DialogParameters
        {
            ["Employee"] = model,
            ["UserAction"] =  action as object,
            ["Title"] = title,
            ["LoginUser"] = _loginUser
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<EmployeeDialog>(title, parameters, _dialogOptions);
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