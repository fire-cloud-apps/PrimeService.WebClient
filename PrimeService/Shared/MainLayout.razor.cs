using System.Text.Json;
using MudBlazor;

namespace FireCloud.WebClient.PrimeService.Shared
{
    public partial class MainLayout
    {
        private MudTheme _theme = new();
        private bool _isDarkMode = true;
        bool _drawerOpen = true;
        private MudText _mudHeaderText;
        private string _txtCompanyName;
        private string _title = "Prime Ser";
        private MudText _mudVersionText;
        private string _version;
        
        private SwitchTheme _switchTheme = new SwitchTheme()
        {
            Color = Color.Success,
            Name = "Dark",
            Title = "Off"
        };

        protected override void OnInitialized()
        {
            Console.WriteLine($"Dark Or Light : {_isDarkMode}");
            _txtCompanyName = _title;
            _version = _appSettings.Version;
        }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
            _txtCompanyName = _drawerOpen ? _title : string.Empty; // "PS";
            _version = _drawerOpen ? _appSettings.Version : string.Empty; // "PS";
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
    }
    public class SwitchTheme
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public Color Color { get; set; }
        public Color IconColor { get; set; }
    }
}
