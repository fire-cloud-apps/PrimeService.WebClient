
using FC.PrimeService.Common.Settings.Dialog;
using MudBlazor;
using PrimeService.Model;

namespace FC.PrimeService.Common.Settings;

public partial class SettingsList
{
    private IList<SettingsMenu> _settingsMenus;
    private string _comingSoon = "Coming Soon";
    private User _loginUser;
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small, 
        FullWidth = false,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    protected override async Task OnInitializedAsync()
    {
        _settingsMenus = new List<SettingsMenu>();
        _settingsMenus.Add(CompanySettings());
        _settingsMenus.Add(TicketSettings());
        _settingsMenus.Add(PaymentsSettings());
        _settingsMenus.Add(FormsSettings());
        _loginUser = await _localStore.GetItemAsync<User>("user");
        StateHasChanged();
    }

    #region Company/General Settings
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
                    Title = "License",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "In which license your Organization claimed.",
                    Icon = @Icons.TwoTone.Stars,
                    Disabled = false,
                    Link = $"/SettingsView?viewId=License"
                },
                new SettingsItem()
                {
                    Title = "Sequence",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Default Sequence Number Generation Settings, You can set 'Prefix', 'Suffix', 'Separator' & 'Batch No' Settings",
                    Icon = Icons.Filled.FormatListNumbered,
                    Link = $"/SettingsView?viewId=Sequence"
                    
                },

                // new SettingsItem()
                // {
                //     Title = "Documents",
                //     ButtonColor = Color.Default,
                //     IconColor = Color.Default,
                //     ToolTip = "Document uploaded into the system",
                //     Disabled = true,
                //     Icon = @Icons.Filled.InsertDriveFile
                // },
                // new SettingsItem()
                // {
                //     Title = "Integration",
                //     ButtonColor = Color.Default,
                //     IconColor = Color.Default,
                //     ToolTip = "3rd Party tool integrated into this platform",
                //     Disabled = true,
                //     Icon = @Icons.TwoTone.IntegrationInstructions
                // },
                
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
                new SettingsItem()
                {
                    Title = "Category",
                    ButtonColor = Color.Default,
                    IconColor = Color.Default,
                    ToolTip = "Service Category",
                    Icon = @Icons.TwoTone.Air,
                    Link = $"/SettingsView?viewId=Category"
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
                     Title = "Client Type",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Defining Client Types",
                     Icon = @Icons.TwoTone.GroupWork,
                     Link = $"/SettingsView?viewId=Client-Type"
                 },
                 new SettingsItem()
                 {
                     Title = "Product Category",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Product Category",
                     Icon = @Icons.TwoTone.Category,
                     Link = $"/SettingsView?viewId=PCategory"
                 },
                 new SettingsItem()
                 {
                     Title = "Tax",
                     ButtonColor = Color.Default,
                     IconColor = Color.Default,
                     ToolTip = "Tax",
                     Icon = @Icons.TwoTone.Percent,
                     Link = $"/SettingsView?viewId=Tax"
                 },
                 
                 // new SettingsItem()
                 // {
                 //     Title = "Handbook",
                 //     ButtonColor = Color.Default,
                 //     IconColor = Color.Default,
                 //     ToolTip = "A Handbook/Tag of information for tagging the device",
                 //     Icon = @Icons.TwoTone.Book,
                 //     Disabled = true
                 // },
                 // new SettingsItem()
                 // {
                 //     Title = "Client Field",
                 //     ButtonColor = Color.Default,
                 //     IconColor = Color.Default,
                 //     ToolTip = "Custom Field definition for Client",
                 //     Icon = @Icons.TwoTone.DashboardCustomize,
                 //     Disabled = true
                 // },
                 // new SettingsItem()
                 // {
                 //     Title = "Ticket Type",
                 //     ButtonColor = Color.Default,
                 //     IconColor = Color.Default,
                 //     ToolTip = "Ticket Categorization",
                 //     Icon = @Icons.TwoTone.Article,
                 //     Disabled = true
                 // },
                 // new SettingsItem()
                 // {
                 //     Title = "Fields",
                 //     ButtonColor = Color.Default,
                 //     IconColor = Color.Default,
                 //     ToolTip = "Custom Field Enable/Disable",
                 //     Icon = @Icons.TwoTone.TextFields,
                 //     Disabled = true
                 // },
             }
         };
         return formSettings;
     }

    #endregion
     
    #region Parform Navigation based on 'Title' Methods
     
     private async  Task PerformNavigation(SettingsItem selectedItem)
     {
         DialogOptions options;
         switch (selectedItem.Title)
         {
             case "License":
                 options = new DialogOptions()
                 {
                     MaxWidth = MaxWidth.Large,
                     FullWidth = false,
                     CloseButton = true,
                     CloseOnEscapeKey = true,
                 };
                 await InvokeDialogBox<LicenseDialog>(
                     "Prime Service License", 
                     options, "_loginUser");
                 break;
             case "Default":
                 await InvokeDialogBox<DefaultSettingsDialog>("Ticket Default Settings", _dialogOptions);
                 break;
             case "Profile":
                 options = new DialogOptions()
                 {
                     MaxWidth = MaxWidth.Medium,
                     FullWidth = false,
                     CloseButton = true,
                     CloseOnEscapeKey = true,
                 };
                 await InvokeDialogBox<UserProfileDialog>("User Profile", options, "_loginUser" );
                 break;
             case "Sequence":
                 options = new DialogOptions()
                 {
                     MaxWidth = MaxWidth.Large,
                     FullWidth = false,
                     CloseButton = true,
                     CloseOnEscapeKey = true,
                 };
                 await InvokeDialogBox<SequenceDialog>("Default Sequence Order", options );
                 break;
             default:
                 _navigationManager.NavigateTo(selectedItem.Link);
                 break;
         }
     }

     async Task InvokeDialogBox<T>(string title, DialogOptions options, string parameter = "") where T : Microsoft.AspNetCore.Components.ComponentBase
     {
         IDialogReference result;
         if (string.IsNullOrEmpty(parameter))
         {
             result = DialogService.Show<T>(title: title, options);
         }
         else
         {
             var parameters = new DialogParameters
                 { [parameter] = _loginUser };
             result = DialogService.Show<T>(title: title, parameters, options);
         }

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