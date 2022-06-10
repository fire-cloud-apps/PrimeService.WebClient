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

namespace PrimeService.Utility;
public interface IHttpService
    {
        Task<T> Get<T>(string uri);
        Task<T> GET<T>(string uri);
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
        T result;
        
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        var user = await _localStorageService.GetItemAsync<User>("user");
        Console.WriteLine($"JWT Token : {user.JwtToken}");
        
        #region Setting up the URL & Bearer Token
        var isApiUrl = !request.RequestUri.IsAbsoluteUri;
        if (user != null )
        {
            Console.WriteLine("Sets Header Bearer");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.JwtToken);
        }
        #endregion
        
        using var response = await _httpClient.SendAsync(request);
        var resRes  = await response.Content.ReadAsStringAsync();
        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("logout");
            return default;
        }

        // throw exception on error response
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(resRes);
            var success = Newtonsoft.Json.JsonConvert.DeserializeObject<SuccessResponse<T>>(resRes);
            result = success.data;
            return result;
        }
        else
        {
            Console.WriteLine($"Error Code {response.StatusCode}");
            Console.WriteLine(resRes);
            var error = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(resRes);
            Console.WriteLine(error.ToJson());
            _snackbar.Add($"Server Request Failed. {error.error.exceptionMessage}", Severity.Error);
            return default;
        }
    }
    
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


