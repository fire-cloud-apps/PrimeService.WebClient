using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FC.Common.Domain;
using FC.Common.Domain.Location;
using FireCloud.WebClient.PrimeService.Service;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Utility;
using PrimeService.Utility.Helper;

namespace FC.PrimeService.Common.Settings;

public partial class CompanyUx
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private Company _inputMode = new Company();
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;

    /// <summary>
    /// HTTP Reqeust
    /// </summary>
    private IHttpService _httpService;
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(200);
        
        #region Ajax Call to Get Company Details
        _httpService = new HttpService(_httpClient, _navigationManager, _localStore, _configuration, Snackbar);
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.CompanyApi.GetDetails}";
        var company = await _httpService.GET<Company>(url);
        if (company == null)
        {
            _inputMode = new Company()
            {
                Title =  "PrimeService Default Laptop & Mobile Service",
                Address =
                    "No 602/3, Elcot Economic Zone, Medavakkam High Road, Sholinganallur, Chennai - 600119, Near Sholinganallur",
                Country = new Country() { Name = "India" },
                SecondaryMobile = "91 98589 99956",
                PrimaryMobile = "91 75589 88956",
                BusinessType = "Repair Shop",
                Currency = "Rupee (₹)"
            };
        }
        else
        {
            // company =  Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(accString);
             _inputMode = company;
            Console.WriteLine("CompanyUx Request Success");
        }
        
        #endregion
        
        _loading = false;
        StateHasChanged();
    }

}