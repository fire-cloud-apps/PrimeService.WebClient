using FireCloud.WebClient.PrimeService.Service.QueryString;
using MudBlazor;
using PrimeService.Utility.Helper;

namespace FireCloud.WebClient.PrimeService.Pages;

public partial class Action
{
    private List<BreadcrumbItem> _items = null;
    string? _component = string.Empty;
    private string? _id = string.Empty;
      
    protected override async Task OnInitializedAsync()
    {
        _navigationManager.TryGetQueryString<string>("Component", out _component);
        _navigationManager.TryGetQueryString<string>("Id", out _id);
        Console.WriteLine($"Component : {_component} Id: {_id}");
        BreadCrumSettings crumSettings = BreadCrumNavigation.GetData(_component);
        
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/", icon: Icons.Material.TwoTone.Home),
            new BreadcrumbItem("Parent", href: "/", icon: Icons.Material.TwoTone.SettingsAccessibility),
            new BreadcrumbItem("Child", href: null, disabled: true, icon: Icons.Material.TwoTone.DoubleArrow)
            // new BreadcrumbItem(crumSettings.Parent, href: crumSettings.ParentLink, icon: crumSettings.ParentIcon),
            // new BreadcrumbItem(_component, href: null, disabled: true, icon: Icons.Material.TwoTone.DoubleArrow)
        };
    }   

}