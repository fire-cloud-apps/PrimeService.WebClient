using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using Model = PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Payments.Payment.Dialogs;

public partial class PaymentDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public Model.Payments _Payments { get; set; } 
    private bool _processing = false;
    MudForm form;
    private Model.Payments _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private string _detectedHeight = "450";
    private string _dialogBehaviour = "max-height:{0}px; overflow-y: scroll; overflow-x: hidden;";
    private bool _isReadOnly = false;

    public List< Model.PaymentTags> _paymentTags = new List<Model.PaymentTags>()
    {
        new Model.PaymentTags() {  Title = "Main Location" },
        new Model.PaymentTags() {  Title = "Secondary Location"},
        new Model.PaymentTags() {  Title = "Emergency Location"},
    }; // In reality it should come from API.
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        if (_Payments == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new Model.Payments()
            {
               ExpenseAmount = 2500,
               PaymentCategory = Model.PaymentCategory.Expense,
               PaymentMethod = new Model.PaymentMethods(){ Title = "Cash"},
               Reason = "Product Service",
               PaymentTag = new Model.PaymentTags(){ Title = "Expense Account"},
               TransactionDate = DateTime.Now,
               Who = new Employee(){ User = new User(){ Name = "SRG"}}
            };
            _title = "Add Payment";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _Payments;
            _title = "Edit Payment";
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
            Snackbar.Add("Submited!", Severity.Success);
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine(_outputJson);
        }
    }

    #endregion

    #region PaymentTags Search - Autocomplete

    private async Task<IEnumerable<Model.PaymentTags>> PaymentTag_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _paymentTags;
        }
        return _paymentTags.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion
    
    #region PaymentMethods Search - Autocomplete
    
    public List< Model.PaymentMethods> _paymentMethods = new List<Model.PaymentMethods>()
    {
        new Model.PaymentMethods() {  Title = "Main Location" },
        new Model.PaymentMethods() {  Title = "Secondary Location"},
        new Model.PaymentMethods() {  Title = "Emergency Location"},
    }; // In reality it should come from API.

    private async Task<IEnumerable<Model.PaymentMethods>> PaymentMethod_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _paymentMethods;
        }
        return _paymentMethods.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion

    #region Password Toggle
    bool PasswordVisibility;
    InputType PasswordInput = InputType.Password;
    string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    void TogglePasswordVisibility()
    {
        if (PasswordVisibility)
        {
            PasswordVisibility = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            PasswordVisibility = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }

    #endregion

    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }
}