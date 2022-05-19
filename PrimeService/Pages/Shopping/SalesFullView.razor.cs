using FireCloud.WebClient.PrimeService.Service.QueryString;
using MudBlazor;
using PrimeService.Utility.Helper;

namespace FireCloud.WebClient.PrimeService.Pages.Shopping;

public partial class SalesFullView
{
    private string _id = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _navigationManager.TryGetQueryString<string>("Id", out _id);
        
        Console.WriteLine($"Id : {_id}");
    }
}