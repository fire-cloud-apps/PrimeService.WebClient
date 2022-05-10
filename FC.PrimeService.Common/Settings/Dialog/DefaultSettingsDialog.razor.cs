using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class DefaultSettingsDialog
{
 
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public DefaultSettings _defaultSettings { get; set; } 
    private bool _processing = false;
    MudForm form;
    private DefaultSettings _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    #endregion

    #region Component Initialization
    protected override async Task OnInitializedAsync()
    {
        if (_defaultSettings == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new DefaultSettings();//Initializes an empty object.
            _title = "Default Settings";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _defaultSettings;
            _title = "Default Settings";
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
            Snackbar.Add("Submited!", Severity.Success);
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