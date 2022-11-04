using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;

namespace FC.PrimeService.Tickets.Ticket.ListItems;

public partial class TicketList
{

    
    #region Initialization Load
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        #region Ajax Call Initialized.
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        #endregion
        _loginUser = await _localStore.GetItemAsync<User>("user");
        Utilities.ConsoleMessage($"Login User {_loginUser.AccountId}");
        _loading = false;
        StateHasChanged();
    }
    
    #endregion
}