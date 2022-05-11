using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;

namespace FC.PrimeService.Common.Settings.Dialog;

public partial class UserProfileDialog
{
    #region Global Variables
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool _loading = false;
    private string _title = string.Empty;
    [Parameter] public User _userProfile { get; set; } 
    private bool _processing = false;
    MudForm form;
    private User _inputMode;
    string _outputJson;
    bool success;
    string[] errors = { };
    
    private bool _isReadOnly = false;

    #endregion

    #region Load Async
    protected override async Task OnInitializedAsync()
    {
        if (_userProfile == null)
        {
            //Dialog box opened in "Add" mode
            _inputMode = new User()
            {
                Name = "SR Ganesh Ram",
                Username = "sr.ganeshram",
                Email = "sr.ganeshram@gmail.com",
                Password = "738993390",
            };
            _title = "User Profile";
        }
        else
        {
            //Dialog box opened in "Edit" mode
            _inputMode = _userProfile;
            _title = "User Profile";
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
    
    public static IEnumerable<string> Max10Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 10 < ch?.Length)
            yield return "Max 10 characters";
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