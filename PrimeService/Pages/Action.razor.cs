using FireCloud.WebClient.PrimeService.Service.QueryString;
using MudBlazor;

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
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/", icon: Icons.Material.TwoTone.Home),
            new BreadcrumbItem("Shop", href: "/Shopping", icon: Icons.TwoTone.ShoppingCart),
            new BreadcrumbItem(_component, href: null, disabled: true, icon: Icons.Material.TwoTone.DoubleArrow)
        };
    }   

}