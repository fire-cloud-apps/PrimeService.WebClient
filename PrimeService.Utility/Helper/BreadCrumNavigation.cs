using MudBlazor;
using PrimeService.Model.Settings.Tickets;

namespace PrimeService.Utility.Helper;

public class BreadCrumNavigation
{
    private static Dictionary<string, BreadCrumSettings> _viewDictionary = new Dictionary<string, BreadCrumSettings>()
    {
        //{"Key","Value"}
        { "", new BreadCrumSettings() { Parent = "", Child = "", ParentIcon = Icons.TwoTone.Home, ParentLink = "/" } },

        {
            "PT",
            new BreadCrumSettings()
            {
                Parent = "Inventory", Child = "Product Transaction", ParentIcon = Icons.TwoTone.History,
                ParentLink = "/View?viewId=Product"
            }
        },
        {
            "Product",
            new BreadCrumSettings()
            {
                Parent = "Inventory", Child = "Product", ParentIcon = Icons.TwoTone.Inventory2,
                ParentLink = "/View?viewId=Product"
            }
        },
        {
            "Client",
            new BreadCrumSettings()
            {
                Parent = "Clients", Child = "Client", ParentIcon = Icons.TwoTone.EmojiPeople,
                ParentLink = "/View?viewId=Client"
            }
        },
    };

    public static BreadCrumSettings GetData(string componentId)
    {
        return _viewDictionary[componentId];
    }
}

public class BreadCrumSettings
{
    public string Parent { get; set; }
    public string ParentIcon { get; set; }
    public string ParentLink { get; set; }
    public string Child { get; set; }
}

