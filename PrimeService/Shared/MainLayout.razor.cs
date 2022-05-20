using System.Text.Json;
using MudBlazor;
using MudBlazor.Utilities;

namespace FireCloud.WebClient.PrimeService.Shared
{
    public partial class MainLayout
    {
        private MudTheme _theme = new MudTheme()
        {
            //Ref: https://mudblazor.com/features/colors#material-colors-list-of-material-colors
            //Lite Palette
            // Palette = new Palette()
            // {
            //     Primary = Colors.Amber.Default,
            //     AppbarBackground = Colors.Amber.Default,
            // },
            //Dark Palette
            // PaletteDark = new Palette()
            // {
            //     Primary = Colors.Amber.Default,
            //     AppbarBackground = Colors.Amber.Default,
            // },
        };
        private bool _isDarkMode = true;
        bool _drawerOpen = true;
        
        private MudText _mudHeaderText;
        private string _txtCompanyName;
        private string _title = "Prime Ser";
        private MudText _mudVersionText;
        private string _version;
        
        
        #region Property

        public bool DrawerOpen
        {
            get
            {
                return _drawerOpen;
            }
            set
            {
                _drawerOpen = value;
                _txtCompanyName = _drawerOpen ? _title : string.Empty; // if, drawer open assign title, else set as empty.
                _version = _drawerOpen ? _appSettings.Version : string.Empty; // if drawer is open, version is displayed, else version is not set.
            }
        }


        #endregion
        
        
        private SwitchTheme _switchTheme = new SwitchTheme()
        {
            Color = Color.Success,
            Name = "Dark",
            Title = "Off"
        };

        protected override void OnInitialized()
        {
            Console.WriteLine($"Dark Or Light : {_isDarkMode}");
            FindDevice();
            _txtCompanyName = _title;
            _version = _appSettings.Version;
            StateHasChanged();
        }

        void DrawerToggle()
        {
            DrawerOpen = !DrawerOpen;//To toggle.
        }

        #region Enable Dark Theme
        private bool ThemeChanged
        {
            get; set;
        }
        
        public void OnToggledChanged(bool toggled)
        {
            // Because variable is not two-way bound, we need to update it ourself
            ThemeChanged = toggled;
            _isDarkMode = toggled;
            if (_isDarkMode)
            {
                _switchTheme = new SwitchTheme()
                {
                    Color = Color.Success,
                    Title = "On",
                    Name = "Dark",
                    IconColor = Color.Success
                };
            }
            else
            {
                _switchTheme = new SwitchTheme()
                {
                    Color = Color.Warning,
                    Title = "Off",
                    Name = "Lite",
                    IconColor = Color.Warning
                };
            }
            Console.WriteLine($"Is DarkMode {_isDarkMode}");
            Console.WriteLine($"{JsonSerializer.Serialize(_switchTheme)}");
        }
        #endregion

        #region Determine Device Type

        private string isDevice { get; set; }
        private bool mobile { get; set; }
        public async Task FindDevice()
        {
            mobile = await jsRuntime.InvokeAsync<bool>("isDevice",null);
            Console.WriteLine($"Device Type: {mobile}");
            isDevice = mobile ? "Mobile" : "Desktop";
        }

        #endregion

        #region MudTheme Manager
        //Not in use currently
        // private ThemeManagerTheme _themeManager = new ThemeManagerTheme();
        // public bool _themeManagerOpen = false;
        //
        // void OpenThemeManager(bool value)
        // {
        //     _themeManagerOpen = value;
        // }
        //
        // void UpdateTheme(ThemeManagerTheme value)
        // {
        //     _themeManager = value;
        //     StateHasChanged();
        // }

        

        #endregion
    }
    public class SwitchTheme
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public Color Color { get; set; }
        public Color IconColor { get; set; }
    }
}
