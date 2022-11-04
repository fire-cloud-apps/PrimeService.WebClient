using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Utility.Helper;

namespace PrimeService.Utility;

public interface IHttpService
{
    Task<T> GET<T>(string uri);
    Task<T> POST<T>(string uri, object value);
    Task<T> PUT<T>(string uri, object value);
    Task<T> DELETE<T>(string uri);

    Task<T> Get<T>(string uri);
    Task<T> Post<T>(string uri, object value);

}

public class HttpService : IHttpService
{
    #region Global Variables
    private HttpClient _httpClient;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    private IConfiguration _configuration;
    private ISnackbar _snackbar;
    

    #endregion

    #region Constructor
    public HttpService(
        HttpClient httpClient,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService,
        IConfiguration configuration,
        ISnackbar snackbar = null
    )
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
        _configuration = configuration;
        _snackbar = snackbar;
    }
    #endregion
    
    public async Task<T> Get<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        return await SendRequest<T>(request);
    }

    public async Task<T> GET<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        #region Setting up the URL & Bearer Token

        await SetAuthenticationHeader<T>(request);

        #endregion
        
        return await HTTPStatusHandler<T>(request);

    }
    
    public async Task<T> POST<T>(string uri,object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);

        #region Setting up the URL & Bearer Token

        await SetAuthenticationHeader<T>(request);

        #endregion
        
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        
        return await HTTPStatusHandler<T>(request);
    }

    public async Task<T> DELETE<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        
        #region Setting up the URL & Bearer Token
        await SetAuthenticationHeader<T>(request);
        #endregion
        
        return await HTTPStatusHandler<T>(request);
    }
    
    public async Task<T> PUT<T>(string uri,object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, uri);

        #region Setting up the URL & Bearer Token
        await SetAuthenticationHeader<T>(request);
        #endregion
        
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        
        return await HTTPStatusHandler<T>(request);
    }


    #region Helper Method

    private  async Task SetAuthenticationHeader<T>(HttpRequestMessage request)
    {
        var user = await _localStorageService.GetItemAsync<User>("user");
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        //Utilities.ConsoleMessage($"JWT Token : {user.JwtToken}");
        
        if (user != null)
        {
            //Utilities.ConsoleMessage("Sets Header Bearer");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.JwtToken);
        }
    }

    private async Task<T> HTTPStatusHandler<T>(HttpRequestMessage request)
    {
        T result = default;
        using var response = await _httpClient.SendAsync(request);
        var resRes = await response.Content.ReadAsStringAsync();
        //Utilities.ConsoleMessage($"Status Code :{response.StatusCode}");
        
        if (response.IsSuccessStatusCode)
        {
            Utilities.ConsoleMessage($"Success State.");
            //Utilities.ConsoleMessage(resRes);
            var success = Newtonsoft.Json.JsonConvert.DeserializeObject<SuccessResponse<T>>(resRes);
            result = success.data;
            //Utilities.ConsoleMessage(result.ToJson());
        }
        else
        {
            Utilities.ConsoleMessage($"Error/UnAuthorized State.");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    _navigationManager.NavigateTo("logout");
                    result = default;
                    break;
                case HttpStatusCode.BadRequest:
                default:
                    result = ErrorRequestHandler<T>(response, resRes);
                    break;
            }
        }
        return result;
    }

    private T? ErrorRequestHandler<T>(HttpResponseMessage response, string resRes)
    {
        T? result;
        Utilities.ConsoleMessage($"Error Code {response.StatusCode}");
        //Utilities.ConsoleMessage(resRes);
        var error = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(resRes);
        //Utilities.ConsoleMessage(error.ToJson());
        _snackbar.Add($"Server Request Failed. {error.error.exceptionMessage}", Severity.Error);
        result = default;
        return result;
    }
    
    #endregion
    public async Task<T> Post<T>(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await SendRequest<T>(request);
    }

    // helper methods
    private async Task<T> SendRequest<T>(HttpRequestMessage request)
    {
        // add jwt auth header if user is logged in and request is to the api url
        var user = await _localStorageService.GetItemAsync<User>("user");
        var isApiUrl = !request.RequestUri.IsAbsoluteUri;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.JwtToken);

        using var response = await _httpClient.SendAsync(request);

        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("logout");
            return default;
        }

        // throw exception on error response
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error["message"]);
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
    
}

public class Error
{
    public string exceptionMessage { get; set; }
}

public class ErrorResponse
{
    public bool isError { get; set; }
    public Error error { get; set; }
}
public class SuccessResponse<T>
{
    public string message { get; set; }
    public T data { get; set; }
}


