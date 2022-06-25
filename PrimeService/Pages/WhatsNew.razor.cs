using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FireCloud.WebClient.PrimeService.Pages;

public partial class WhatsNew
{
    [Inject] ISnackbar Snackbar { get; set; }
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    private ReleaseNotes _releaseNotes;
    
    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        string url = $"{_navigationManager.BaseUri}/json/release-note.json";
        _releaseNotes = await _httpService.Get<ReleaseNotes>(url);
        
        #endregion
        StateHasChanged();
    }
    #endregion
    
}