using PrimeService.Model.Settings.Tickets;
using MudBlazor;

namespace PrimeService.Utility.Helper;

public static class Utilities
{
    /// <summary>
    /// Message to the client user.
    /// </summary>
    /// <param name="snackbar"></param>
    /// <param name="msg"></param>
    /// <param name="severity"></param>
    /// <param name="variant"></param>
    public static void SnackMessage(ISnackbar snackbar, string msg, 
        Severity severity = Severity.Success, Variant variant = Variant.Filled)
    {
        //Message
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        snackbar.Configuration.SnackbarVariant = variant;
        //Snackbar.Configuration.VisibleStateDuration  = 2000;
        //Can also be done as global configuration. Ref:
        //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
        snackbar.Add(msg, severity);
    }

    /// <summary>
    /// Delete Confirmation Message Box
    /// </summary>
    /// <param name="DialogService">Injection Dialog Service</param>
    /// <returns>Can Delete or not 'bool' value.</returns>
    public static async Task<bool> DeleteConfirm(IDialogService DialogService)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Heads Up! Are you sure?. Deleting can not be undone!", 
            yesText:"Delete!", cancelText:"Cancel");
        var canDelete = result == null ? false : true;
        Console.WriteLine( $"Can Delete : {canDelete}");
        return canDelete;
    }

    /// <summary>
    /// Write to console message if 'DEBUG' Mode
    /// </summary>
    /// <param name="msg">Message to Write in console</param>
    public static void ConsoleMessage(string msg)
    {
        #if DEBUG
        Console.WriteLine(msg);
        #endif
    }
}