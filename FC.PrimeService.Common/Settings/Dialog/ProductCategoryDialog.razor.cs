using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Shopping;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class ProductCategoryDialog
{
    #region Initialization
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;

    #region Dialog Parameters
    [Parameter] public ProductCategory ProductCategory { get; set; } 
    [Parameter] public string Title { get; set; }
    [Parameter] public UserAction UserAction { get; set; }
    #endregion
    
    private bool _processing = false;
    MudForm form;
    private ProductCategory _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        Utilities.ConsoleMessage($"ProductCategory - User Action : {UserAction}");
        if (UserAction == UserAction.ADD)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new ProductCategory();//Initializes an empty object.
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = ProductCategory;
        }
        
    }
    #endregion
   
    #region Submit, Delete, Cancel Button with Animation

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            if (await SubmitAction(UserAction))
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Product Category Saved!");
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Utilities.ConsoleMessage(_outputJson);
        }
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        ProductCategory responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductCategoryApi.Create}";
                responseModel = await _httpService.POST<ProductCategory>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductCategoryApi.Update}";
                responseModel = await _httpService.PUT<ProductCategory>(url, _inputMode);
                result = (responseModel != null);
                break;
            case UserAction.DELETE:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductCategoryApi.Delete}";
                url = string.Format(url, _inputMode.Id);
                result = await _httpService.DELETE<bool>(url);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Product Category JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }
    private void Cancel()
    {
        MudDialog.Cancel();
    }

    async Task GetFakeData()
    {
        _loading = true;
        string url = string.Empty;
        url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductCategoryApi.Fake}";
        _inputMode = await _httpService.GET<ProductCategory>(url);
        _loading = false;
    }

    async Task Delete()
    {
        var canDelete = await Utilities.DeleteConfirm(DialogService);
        if (canDelete)
        {
            await SubmitAction(UserAction.DELETE);
            Utilities.SnackMessage(Snackbar, "ProductCategory Deleted!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    #endregion
}