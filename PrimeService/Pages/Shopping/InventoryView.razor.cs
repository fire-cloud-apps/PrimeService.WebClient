using FireCloud.WebClient.PrimeService.Service.QueryString;
using MudBlazor;
using PrimeService.Utility.Helper;

namespace FireCloud.WebClient.PrimeService.Pages.Shopping;

public partial class InventoryView
{
    private List<BreadcrumbItem> _items = null;
    string _viewId = string.Empty;
    private string _id = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        _navigationManager.TryGetQueryString<string>("viewId", out _viewId);
        _navigationManager.TryGetQueryString<string>("Id", out _id);
        
        Console.WriteLine($"ViewId : {_viewId} - ObjectId : {_id}");
        
        BreadCrumSettings crumSettings = BreadCrumNavigation.GetData(_viewId);
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/", icon: Icons.Material.TwoTone.Home),
            new BreadcrumbItem(crumSettings.Parent, href: crumSettings.ParentLink, icon: crumSettings.ParentIcon),
            new BreadcrumbItem(crumSettings.Child, href: null, disabled: true, icon: Icons.Material.TwoTone.DoubleArrow)
        };
        var icon = Icons.TwoTone.Home;
    }
}