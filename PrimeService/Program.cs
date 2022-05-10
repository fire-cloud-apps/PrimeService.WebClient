using System.Text.Json;
using Blazored.LocalStorage;
using FireCloud.WebClient.PrimeService.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage.JsonConverters;
using Blazored.LocalStorage.Serialization;
using Blazored.LocalStorage.StorageOptions;
using Blazor.Extensions.Logging;
using FireCloud.WebClient.PrimeService.Service.Helper;
using App = FireCloud.WebClient.PrimeService.App;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//https://localhost:7086
#region Dependency injection, Logging & Scope

builder.Services.AddLogging(builder => builder
                .SetMinimumLevel(LogLevel.Information));

builder.Services
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IHttpService, HttpService>();
    //.AddScoped<ILocalStorageService, LocalStorageService>();

builder.Services.AddBlazoredLocalStorage();

#endregion

#region App Settings & HTTP Scope 
//appsettings.json is case-sensitive file
// configure http client
builder.Services.AddScoped(sp => new HttpClient
{
    //BaseAddress = new Uri("https://gorest.co.in/")
    BaseAddress = new Uri(builder.Configuration["App:AuthUrl"])
});

var appConfig = builder.Configuration.Get<AppSettings>();
builder.Services.AddSingleton(appConfig);
Console.WriteLine($"API URL : {builder.Configuration["App:AuthUrl"]}");

#endregion

#region Build & Initialize

builder.Services.AddMudServices();
var host = builder.Build();

var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
await authenticationService.Initialize();


#endregion

await host.RunAsync();
