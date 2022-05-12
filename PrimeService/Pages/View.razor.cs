using FireCloud.WebClient.PrimeService.Service.QueryString;
using MudBlazor;

namespace FireCloud.WebClient.PrimeService.Pages;

public partial class View
{
    private List<BreadcrumbItem> _items = null;
    string _viewId = string.Empty;
   
    protected override async Task OnInitializedAsync()
    {
        _navigationManager.TryGetQueryString<string>("viewId", out _viewId);
        Console.WriteLine($"ViewId : {_viewId}");
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/", icon: Icons.Material.TwoTone.Home),
            new BreadcrumbItem("Shop", href: "/Shopping", icon: Icons.TwoTone.ShoppingCart),
            new BreadcrumbItem(_viewId, href: null, disabled: true, icon: Icons.Material.TwoTone.DoubleArrow)
        };
    }   
}