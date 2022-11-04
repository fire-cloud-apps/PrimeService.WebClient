using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Shopping;
using PrimeService.Model.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class ServiceTypeList
{
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
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
    private MudTable<ServiceType> _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = null;
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<ServiceType>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<ServiceType>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }

    /// <summary>
    /// Do Ajax call to get 'ServiceType' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>ServiceType Data.</returns>
    private async Task<ResponseData<ServiceType>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(_searchString)) ? string.Empty : _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Title" : state.SortLabel,
            SearchField = "Title",
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<ServiceType>>(url, pageMetaData);
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
    
    private async Task Set2Default(MouseEventArgs arg)
    {
        var canReset = await Utilities.DeleteConfirm(DialogService, message:"Are you sure? do you want to Reset?", yesTxt:"Reset!");
        if (canReset)
        {
            await SubmitAction(UserAction.RESET);
            OnSearch(string.Empty);
            Utilities.SnackMessage(Snackbar, "Reset Completed!", Severity.Warning);
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Reset Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    
    private DialogOptions _dialogOptions = new ()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private async Task OpenEditDialog(ServiceType model)
    {
        Utilities.ConsoleMessage(JsonSerializer.Serialize(model));
        await InvokeDialog("Edit Service Type", UserAction.EDIT, model:model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("Add Service Type",UserAction.ADD);
    }
    
    private async Task InvokeDialog(string title, 
        UserAction action = UserAction.ADD, ServiceType model = null)
    {
        var parameters = new DialogParameters
        {
            ["ServiceType"] = model,
            ["UserAction"] =  action as object,
            ["Title"] = title
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<ServiceTypeDialog>(title, parameters, _dialogOptions);
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

    #region Submit
    private async Task Submit( ServiceType model)
    {
        _processing = true;
        await form.Validate();
        if (form.IsValid)
        {
            _inputMode = model;
            var isSuccess = await SubmitAction(UserAction.EDIT);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "ServiceType Saved!");
            }
            Utilities.ConsoleMessage(model.ToJson());
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Utilities.ConsoleMessage(_outputJson);
        }
        _processing = false;
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        ServiceType responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.RESET:
                _loading = true;
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Reset2Default}";
                var restModel = await _httpService.POST<ResponseData<ServiceType>>(url, _inputMode);
                //var responseModel = await httpService.POST<ResponseData<Client>>(url, pageMetaData);
                result = (restModel != null);
                _loading = false;
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Update}";
                responseModel = await _httpService.PUT<ServiceType>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ServiceTypeApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"ServiceType JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }
    #endregion

    #region Expansion

    private RenderFragment _panelContent;
    private ServiceType _inputMode = new ServiceType()
    {
        Cost = 250,
        Price = 150,
        Title = "General Service",
        Type = TicketType.GeneralService,
        Warranty = 90
    };

    private MudExpansionPanel _panel;

    TicketType _selectedType;
    private async Task ExpandedChanged(bool newVal)
    {
        if (newVal)
        {
            await Task.Delay(600);
            Utilities.ConsoleMessage($"Selected Panel Tag : {_panel.Tag}");
            Utilities.ConsoleMessage($"Selected Ticket Type : {_selectedType}");
        }
        else
        {
            // Reset after a while to prevent sudden collapse.
            Task.Delay(350).ContinueWith(t => _panelContent = null).AndForget(); 
        }
    }

    



    #endregion

    
}