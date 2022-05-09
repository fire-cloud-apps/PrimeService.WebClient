using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model.Settings;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class  WorkLocationDialog
{
    #region Initialization
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public WorkLocation _WorkLocation { get; set; } 
    private bool _processing = false;
    MudForm form;
    private WorkLocation _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    private bool _isReadOnly = false;
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        if (_WorkLocation == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new WorkLocation();//Initializes an empty object.
            _title = "Add Work Location";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _WorkLocation;
            _title = "Edit Work Location";
        }
        
    }
    
    private void Cancel()
    {
        MudDialog.Cancel();
    }
    
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
}