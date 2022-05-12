using System.Text.Json;
using FireCloud.WebClient.PrimeService.Service.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class EmployeeDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public Employee _employee { get; set; } 
    private bool _processing = false;
    MudForm form;
    private Employee _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private string _detectedHeight = "450";
    private string _dialogBehaviour = "max-height:{0}px; overflow-y: scroll; overflow-x: hidden;";
    private bool _isReadOnly = false;

    public List<WorkLocation> _WorkLocations = new List<WorkLocation>()
    {
        new WorkLocation() {  Title = "Main Location", Address = "Chennai", Phone = "452563632"},
        new WorkLocation() {  Title = "Secondary Location", Address = "Trv", Phone = "85969323"},
        new WorkLocation() {  Title = "Emergency Location", Address = "Bombay", Phone = "747523669"},
    }; // In reality it should come from API.
    
    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        if (_employee == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new Employee()
            {
                User = new User(),
                WorkLocation = new WorkLocation()
            };
            _title = "Add Employee";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _employee;
            _title = "Edit Employee";
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

    #region WorkLocation Search - Autocomplete

    private async Task<IEnumerable<WorkLocation>> WorkLocation_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _WorkLocations;
        }
        return _WorkLocations.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
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