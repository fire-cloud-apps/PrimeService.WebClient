using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using PrimeService.Model;
using FireCloud.WebClient.PrimeService.Service.QueryString;

namespace FireCloud.WebClient.PrimeService.Pages.Auth;

public partial class SuccessLogin
{
    #region Initialization
    private string _jwt;
    private string _message;
    private string _encode;
    IList<Claim> _claimList;
    #endregion
    
    protected  async override void OnInitialized()
    {
        _claimList = GetClaims();//Gets the list of claims from the query string by decoding JWT Token
        User user = new User();
        SetUserClaim(user);//Set the user details from the Claim data.
        #region Debugging
#if DEBUG
        string jsonString = JsonSerializer.Serialize(user);
        Console.WriteLine(jsonString);
#endif
        #endregion
        await _localStore.SetItemAsync("user", user);//Store the user for login purpose.

        #region Validate user and redirect to main page.
        User userInfo = await _localStore.GetItemAsync<User>("user");
        if (userInfo != null)
        {
            _navigationManager.NavigateTo("Index");
        }
        else
        {
            _navigationManager.NavigateTo("Error?code=500");
        }
        #endregion
    }

    private IList<Claim> GetClaims()
    {
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        _navigationManager.TryGetQueryString<string>("jwt", out _jwt);
        _navigationManager.TryGetQueryString<string>("msg", out _message);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(_jwt);
        _claimList = token.Claims.ToList();//Set the claim list from the token.
        return _claimList;
    }

    private void SetUserClaim(User user)
    {
        foreach (var claim in _claimList)
        {
            switch (claim.Type)
            {
                case "Id":
                    user.Id = claim.Value;
                    break;
                case "Name":
                    user.Name = claim.Value;
                    break;
                case "Email":
                    user.Email = claim.Value;
                    break;
                case "AccountId":
                    user.AccountId = claim.Value;
                    break;
                case "ConnectionKey":
                    user.ConnectionKey = claim.Value;
                    break;
                case "Picture":
                    user.Picture = claim.Value;
                    break;
                default:
                    break;
            }
        }
    }
}