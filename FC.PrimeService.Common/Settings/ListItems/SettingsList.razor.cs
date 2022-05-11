using FC.PrimeService.Common.Settings.Dialog;
using MudBlazor;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class SettingsList
{
    private IList<SettingsMenu> _settingsMenus;
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small, 
        FullWidth = false,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    protected override void OnInitialized()
    {
        _settingsMenus = new List<SettingsMenu>();
        _settingsMenus.Add(CompanySettings());
        _settingsMenus.Add(TicketSettings());
        _settingsMenus.Add(PaymentsSettings());
        _settingsMenus.Add(FormsSettings());
        StateHasChanged();
    }

    #region Company Settings
    private SettingsMenu CompanySettings()
    {
        SettingsMenu companySettings = new SettingsMenu()
        {
            Title = "Company Settings",
            Items = new List<SettingsItem>()
            {
                new SettingsItem()
                {
                    Title = "General",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Company Profile Details Service",
                    Icon = Icons.Filled.Task,
                    Link = $"/SettingsView?viewId=Company"
                },
                new SettingsItem()
                {
                    Title = "Location",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Company HQ Location",
                    Icon = Icons.Filled.LocationOn,
                    Link = $"/SettingsView?viewId=Location"
                },
                new SettingsItem()
                {
                    Title = "Employee",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Employee Service",
                    Icon = @Icons.Filled.Group,
                    Link = $"/SettingsView?viewId=Employee"
                },
                new SettingsItem()
                {
                    Title = "Profile",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Your Profile",
                    Disabled = false,
                    Icon =@Icons.TwoTone.AccountBox,
                    Link = $"/SettingsView?viewId=Profile"
                },
                new SettingsItem()
                {
                    Title = "Documents",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Document uploaded into the system",
                    Disabled = false,
                    Icon = @Icons.Filled.InsertDriveFile
                },
                new SettingsItem()
                {
                    Title = "Integration",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "3rd Party tool integrated into this platform",
                    Disabled = false,
                    Icon = @Icons.TwoTone.IntegrationInstructions
                },
                new SettingsItem()
                {
                    Title = "License",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "In which license your Organization claimed.",
                    Icon = @Icons.TwoTone.Stars,
                    Disabled = false,
                    Link = $"/SettingsView?viewId=License"
                },
                
            }
        };
        return companySettings;
    }
    
    #endregion
   
    #region Ticket Settings

    private SettingsMenu TicketSettings()
    {
        SettingsMenu settings = new SettingsMenu()
        {
            Title = "Ticket Settings",
            Items = new List<SettingsItem>()
            {
                new SettingsItem()
                {
                    Title = "Default",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Ticket Default Values",
                    Icon = @Icons.TwoTone.StickyNote2
                },
                // new SettingsItem()
                // {
                //     Title = "Notification",
                //     ButtonColor = Color.Default,
                //     IconColor = Color.Default,
                //     ToolTip = "Enable/Disable Notification",
                //     Icon = @Icons.TwoTone.NotificationsActive
                // },
                new SettingsItem()
                {
                    Title = "Status",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Ticket Status Definition",
                    Icon = @Icons.TwoTone.FeaturedPlayList,
                    Link = $"/SettingsView?viewId=Status"
                },
                new SettingsItem()
                {
                    Title = "Services",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Services Provided by us",
                    Icon = @Icons.TwoTone.HomeRepairService,
                    Link = $"/SettingsView?viewId=Services"
                }
            }
        };
        return settings;
    }
    
    #endregion

    #region  Payment Settings
    private SettingsMenu PaymentsSettings()
    {
        SettingsMenu settings = new SettingsMenu()
        {
            Title = "Payment Settings",
            Items = new List<SettingsItem>()
            {
                new SettingsItem()
                {
                    Title = "Account",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Payment Tags for ease of use",
                    Icon = @Icons.TwoTone.MonetizationOn,
                    Link = $"/SettingsView?viewId=Account"
                    
                },
                new SettingsItem()
                {
                    Title = "Methods",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Various ways of payments collected by the clients.",
                    Icon = @Icons.TwoTone.CreditCard,
                    Link = $"/SettingsView?viewId=PaymentMethod"
                },
            }
        };
        return settings;
    }

    #endregion

    #region Forms

    private SettingsMenu FormsSettings()
     {
         SettingsMenu formSettings = new SettingsMenu()
         {
             Title = "Form Settings",
             Items = new List<SettingsItem>()
             {
                 new SettingsItem()
                 {
                     Title = "Ticket Type",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Ticket Categorization",
                     Icon = @Icons.TwoTone.Article
                 },
                 new SettingsItem()
                 {
                     Title = "Fields",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Custom Field Enable/Disable",
                     Icon = @Icons.TwoTone.TextFields
                 },
                 new SettingsItem()
                 {
                     Title = "Client Type",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Defining Client Types",
                     Icon = @Icons.TwoTone.GroupWork
                 },
                 new SettingsItem()
                 {
                     Title = "Client Field",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Custom Field definition for Client",
                     Icon = @Icons.TwoTone.DashboardCustomize
                 },
                 new SettingsItem()
                 {
                     Title = "Handbook",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "A Handbook/Tag of information for tagging the device",
                     Icon = @Icons.TwoTone.Book
                 },
             }
         };
         return formSettings;
     }

    #endregion
     
     #region Helper Methods
     
     private async  Task PerformNavigation(SettingsItem selectedItem)
     {
         switch (selectedItem.Title)
         {
             case "License":
                 await InvokeDialogBox<LicenseDialog>("Prime Service License", _dialogOptions);
                 break;
             case "Default":
                 await InvokeDialogBox<DefaultSettingsDialog>("Ticket Default Settings", _dialogOptions);
                 break;
             case "Profile":
                 DialogOptions options = new DialogOptions()
                 {
                     MaxWidth = MaxWidth.Medium,
                     FullWidth = false,
                     CloseButton = true,
                     CloseOnEscapeKey = true,
                 };
                 await InvokeDialogBox<UserProfileDialog>("User Profile", options );
                 break;
             // case "Account": //This is for testing should be removed it should call web page
             //     options = new DialogOptions()
             //     {
             //         MaxWidth = MaxWidth.Large,
             //         FullWidth = false,
             //         CloseButton = true,
             //         CloseOnEscapeKey = true,
             //     };
             //     await InvokeDialogBox<PaymentTagDialog>("Payment Tag", options );
             //     break;
             default:
                 _navigationManager.NavigateTo(selectedItem.Link);
                 break;
         }
        
     }

     async Task InvokeDialogBox<T>(string title, DialogOptions options) where T : Microsoft.AspNetCore.Components.ComponentBase
     {
         var result = DialogService.Show<T>(title: title, options);
         if (!result.Result.IsCanceled)
         {
             //some action.
         }
     }

     #endregion
}

public class SettingsMenu
{
    public string Title { get; set; }
    public IList<SettingsItem> Items { get; set; }
}

public class SettingsItem
{
    public string Title { get; set; }
    public Color IconColor { get; set; }
    public string Icon { get; set; }
    public Color ButtonColor { get; set; }
    public string ToolTip { get; set; }
    public string Link { get; set; } = "/SettingsView?viewId=ytb";
    public bool Disabled { get; set; } = false;
} 