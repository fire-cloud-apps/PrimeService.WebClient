using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using FireCloud.WebClient.PrimeService.Model;

namespace FireCloud.WebClient.PrimeService.Service;
public interface IAuthenticationService
{
    User User { get; }
    Task Initialize();
    Task<User> Login(User user);
    Task Logout();

    bool IsAuthenticated();
}

public class AuthenticationService : IAuthenticationService
{
    private IHttpService _httpService;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    

    public User User { get; private set; }

    public AuthenticationService(
        IHttpService httpService,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService
    ) {
        _httpService = httpService;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
    }

    public async Task Initialize()
    {
        //User = await _localStorageService.GetItem<User>("user");
        User = await _localStorageService.GetItemAsync<User>("user");
    }
    
    public async Task<User> Login(User user)
    {
        //Console.WriteLine(user.ToJSON());
        User = await _httpService.Post<User>("/API/UserAuth/FCAuth", 
            new
            {
                user.Username, 
                user.Password, 
                user.DomainURL,
                user.UserType
            });
        await _localStorageService.SetItemAsync<User>("user", User);
        await _localStorageService.SetItemAsStringAsync("authToken", User.JwtToken);
        await _localStorageService.SetItemAsStringAsync("refreshToken", User.RefreshToken);
        return user;
    }

    private bool _isAuthorized = false;

    public bool IsAuthenticated()
    {
        User userInfo = _localStorageService.GetItemAsync<User>("user").Result;
        if (userInfo != null)
        {
            if (string.IsNullOrEmpty(userInfo.Id))
            {
                _isAuthorized = false;
            }
            else
            {
                _isAuthorized = true;
            }
        }

        return _isAuthorized;

    }

    public async Task Logout()
    {
        User = null;
        await _localStorageService.RemoveItemAsync("user");
        _navigationManager.NavigateTo("login");
    }
}