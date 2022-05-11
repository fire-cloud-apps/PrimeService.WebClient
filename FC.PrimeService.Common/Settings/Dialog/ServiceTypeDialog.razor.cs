using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class ServiceTypeDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public ServiceType _ServiceType { get; set; } 
    private bool _processing = false;
    MudForm form;
    private ServiceType _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;

    public List<ServiceCategory> _serviceCategorys = new List<ServiceCategory>()
    {
        new ServiceCategory() { CategoryName = "Repair" },
        new ServiceCategory() { CategoryName = "Maintenance" },
        new ServiceCategory() { CategoryName = "Service" },
        new ServiceCategory() { CategoryName = "Support" },
        
    }; // In reality it should come from API.
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        if (_ServiceType == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new ServiceType()
            {
               Price = 100,
               Cost = 10,
               Title = "Laptop Repair",
               Warranty = 30,
               Category = new ServiceCategory()
               {
                   CategoryName = "Repair"
               }
            };
            _title = "Add Service Type";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _ServiceType;
            _title = "Edit Service Type";
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

    #region ServiceCategory Search - Autocomplete

    private async Task<IEnumerable<ServiceCategory>> ServiceCategory_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _serviceCategorys;
        }
        return _serviceCategorys.Where(x => x.CategoryName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion

    #region Generate Fake
    private Task GetFakeData()
    {
        throw new NotImplementedException();
    } 
    #endregion
    
}