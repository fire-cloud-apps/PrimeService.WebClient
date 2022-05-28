using System.Collections.ObjectModel;
using System.Text.Json;
using FC.PrimeService.Tickets.PaymentDetail.Dialog;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson.Serialization.IdGenerators;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Location;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;

namespace FC.PrimeService.Tickets.Ticket;

public partial class TicketComponent
{
    
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

    #region Fake Data

    private Task GetFakeData()
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region Add Payment Dialog
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Medium,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private async Task InvokeDialog(string parameter, string title, Model.PaymentDetails model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        IDialogReference dialog;

        dialog = DialogService.Show<PaymentDetailsDialog>(title, parameters, _dialogOptions);
        
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    #endregion

    private async Task InvokeAddPaymentDialog()
    {
        await InvokeDialog("_PaymentDetails", "Add Payments", null);

    }
}