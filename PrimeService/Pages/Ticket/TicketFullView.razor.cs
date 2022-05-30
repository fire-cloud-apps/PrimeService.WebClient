using FireCloud.WebClient.PrimeService.Service.QueryString;

namespace FireCloud.WebClient.PrimeService.Pages.Ticket;

public partial class TicketFullView
{
    private string _id = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _navigationManager.TryGetQueryString<string>("Id", out _id);
        
        Console.WriteLine($"Id : {_id}");
    }
}