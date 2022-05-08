using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model.Location;
using PrimeService.Model.Settings;

namespace FC.PrimeService.Common.Settings;

public partial class CompanyUx
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private Account _inputMode = new Account();
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        //for now it is filled as Static value
        _inputMode = new Account()
        {
            BusinessName = "HCL Laptop & Mobile Service",
            Address =
                "No 602/3, Elcot Economic Zone, Medavakkam High Road, Sholinganallur, Chennai - 600119, Near Sholinganallur",
            Country = new Country() { Name = "India" },
            SecondaryMobile = "91 98589 99956",
            PrimaryMobile = "91 75589 88956",
            BusinessType = "Repair Shop",
            Currency = "Rupee (₹)"
        };
        _loading = false;
        StateHasChanged();
    }

}