using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Shopping;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Shopping.Inventory.Dialog;

public partial class AddStockDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public Product Product { get; set; } //This comes from 'Dialog' invoker.
    [Parameter] public User User { get; set; }
    private bool _processing = false;
    MudForm form;
    private ProductTransaction _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    private UserAction UserAction;
    #endregion

    #region Component Initialization
    protected override async Task OnInitializedAsync()
    {
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        Utilities.ConsoleMessage($"Global User {GlobalConfig.LoginUser.ToJson()}");
        
        if (Product == null)
        {
            UserAction = UserAction.ADD;
            //Dialog box opened in "Add" mode
            _inputMode = new ProductTransaction();//Initializes an empty object.
            _title = "Add Stock";
        }
        else
        {
            UserAction = UserAction.EDIT;
            //Dialog box opened in "Edit" mode
            _inputMode = new ProductTransaction()
            {
                ProductId = Product.Id,
                Action = StockAction.In,
                Reason = "Force Stock Add",
                Price = Product.Cost,
                TransactionDate = DateTime.Now,
                Who = GlobalConfig.LoginUser
            };
            _title = $"Add Stock - {Product.Name}";
        }
    }
    #endregion
    
    #region Submit, Delete, Cancel Button with Animation
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            //Todo some animation.
            var isSuccess = await SubmitAction(UserAction);
            if (isSuccess)
            {
                _outputJson = JsonSerializer.Serialize(_inputMode);
                Utilities.SnackMessage(Snackbar, "Product Stock Updated!");
                MudDialog.Cancel();
            }
        }
        else
        {
            _outputJson = "Validation Error occured.";
        }
        Utilities.ConsoleMessage(_outputJson);
    }
    
    async Task<bool> SubmitAction(UserAction action)
    {
        _processing = true;
        string url = string.Empty;
        ProductTransaction responseModel = null;
        bool result = false;
        switch (action)
        {
            case UserAction.ADD:
            case UserAction.EDIT:
                url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.ProductTransactionApi.Create}";
                responseModel = await _httpService.POST<ProductTransaction>(url, _inputMode);
                result = (responseModel != null);
                break;
            default:
                break;
        }
        Utilities.ConsoleMessage($"Executed API URL : {url}, Method {action}");
        Utilities.ConsoleMessage($"Product Transaction JSON : {_inputMode.ToJson()}");
        _processing = false;
        return result;
    }
    
    
    async Task Delete()
    {
        var canDelete = await Utilities.DeleteConfirm(DialogService);
        if (canDelete)
        {
            await SubmitAction(UserAction.DELETE);
            Utilities.SnackMessage(Snackbar, "Client Deleted!", Severity.Warning);
        }
        else
        {
            Utilities.SnackMessage(Snackbar, "Deletion Cancelled!", Severity.Normal);
        }
        StateHasChanged();
    }
    #endregion
}
