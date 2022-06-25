namespace PrimeService.Utility.Helper;

public class MaterialColors
{
    /// <summary>
    /// HTTP Request
    /// </summary>
    private IHttpService _httpService;

    private static Dictionary<string, Dictionary<string, string>>? _dicMudColor = null;
    private string _baseUrl;
    
    public MaterialColors(IHttpService httpService, string baseUrl)
    {
        _httpService = httpService;
        _baseUrl = baseUrl;
    }

    public async Task<string> GetRandomColor()
    {
        string colorCode = "#9E9E9E";//Default Color Code;
        if (_dicMudColor == null)
            await SetMudColors();
        else
        {
            int colorIndex = new Random().Next(_dicMudColor.Count);
            KeyValuePair<string,Dictionary<string,string>> color = _dicMudColor.ElementAt(colorIndex);
            int colorCodeIndex = new Random().Next(color.Value.Count);
            colorCode = color.Value.Values.ElementAt(colorCodeIndex);
        }
        return colorCode;
    }

    private async Task SetMudColors()
    {
        if (_dicMudColor == null)
        {
            string url = $"{_baseUrl}/json/material-colors.json";
            _dicMudColor = await _httpService.Get<Dictionary<string, Dictionary<string,string>>>(url);
            Utilities.ConsoleMessage($"Mud Color from Ajax : {_dicMudColor}");
            Utilities.ConsoleMessage($"Mud Color Count {_dicMudColor.Count}");
            foreach (var color in _dicMudColor)
            {
                Utilities.ConsoleMessage($"Color {color.Key}");
                foreach (var colorCode in color.Value)
                {
                    Utilities.ConsoleMessage($"Color Code: {colorCode.Key}: {colorCode.Value}");
                }
            }
        }
    }
} 