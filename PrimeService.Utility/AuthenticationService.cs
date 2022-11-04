using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Utility.Helper;

namespace PrimeService.Utility;
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
       var userResponse = await _httpService.Post<User>("UserAuth/FCAuth", 
            new
            {
                user.Username, 
                user.Password, 
                user.DomainURL,
                user.UserType
            });
       if (userResponse.IsSuccess)
       {
           User = userResponse;
           User.Password = string.Empty;
           var claims = Utilities.ParseClaimsFromJwt(User.JwtToken);
           
           var auditUser = GetLoginUser(claims);
           GlobalConfig.LoginUser = auditUser;
           await _localStorageService.SetItemAsync<AuditUser>("LoginUser", auditUser);
           await _localStorageService.SetItemAsync<User>("user", User);
           await _localStorageService.SetItemAsStringAsync("authToken", User.JwtToken);
           await _localStorageService.SetItemAsStringAsync("refreshToken", User.RefreshToken);
           //setLogRocketUser
       }
       else
       {
           User = new User()
           {
               IsSuccess = false,
               Message = userResponse.Message
           };
       }
       
        return User;
    }

    private static AuditUser GetLoginUser(IEnumerable<Claim> claims)
    {
        AuditUser auditUser = new AuditUser();
        foreach (var claim in claims)
        {
            switch (claim.Type)
            {
                case "Id":
                    auditUser.UserId = claim.Value;
                    break;
                case "Email":
                    auditUser.EmailId = claim.Value;
                    break;
                case "Name":
                    auditUser.Name = claim.Value;
                    break;
                case "AccountId":
                    auditUser.AccountId = claim.Value;
                    break;
                case "Picture":
                    auditUser.Picture = claim.Value;
                    break;
            }
        }

        return auditUser;
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