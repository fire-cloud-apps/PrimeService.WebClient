using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class PaymentMethodDialog
{
            #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public PaymentMethods _PaymentMethods { get; set; } 
    private bool _processing = false;
    MudForm form;
    private PaymentMethods _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        if (_PaymentMethods == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new PaymentMethods()
            {
                Title = "Cash",
            };
            _title = "Add Payment Method";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _PaymentMethods;
            _title = "Edit Payment Method";
        }
    }
    #endregion
    
    #region Cancel & Close
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    #endregion

    #region Submit Button with Animation
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

    #region Generate Fake
    private Task GetFakeData()
    {
        throw new NotImplementedException();
    } 
    #endregion
}