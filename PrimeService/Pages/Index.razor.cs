using System.Text.Json;
using PrimeService.Model;
using FireCloud.WebClient.PrimeService.Service;

namespace FireCloud.WebClient.PrimeService.Pages;

public partial class Index
{
    private bool loading;
    private IEnumerable<User> users;
    private bool _isAuthenticated;
    private User _userInfo;

    protected override async Task OnInitializedAsync()
    {
        loading = false;
        _userInfo = await _localStore.GetItemAsync<User>("user");

        if (_userInfo != null)
        {
           _isAuthenticated = true;
           Console.WriteLine(JsonSerializer.Serialize(_userInfo));
        }
        loading = true;
        StateHasChanged();
    }
   

}