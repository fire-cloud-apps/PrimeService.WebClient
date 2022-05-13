using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Inventory.Dialog;

public partial class AddStockDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public Product _Product { get; set; } //This comes from 'Dialog' invoker.
    private bool _processing = false;
    MudForm form;
    private Product _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    
    #endregion

    #region Component Initialization
    protected override async Task OnInitializedAsync()
    {
        if (_Product == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new Product();//Initializes an empty object.
            _title = "Add Stock";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _Product;
            _title = $"Add Stock - {_inputMode.Name}";
        }
    }
    #endregion
    
    #region Submit, Cancel Button with Animation
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    
    async Task ProcessSomething()
    {
        _processing = true;
        await Task.Delay(2000);
        _processing = false;
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // //Todo some animation.
            await ProcessSomething();

            //Do server actions.
            _outputJson = JsonSerializer.Serialize(_inputMode);

            //Success Message
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            //Snackbar.Configuration.VisibleStateDuration  = 2000;
            //Can also be done as global configuration. Ref:
            //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
            Snackbar.Add("Submitted!", Severity.Success);
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine(_outputJson);
        }
    }
    
    #endregion

    #region Fake Data
    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }
    #endregion
}