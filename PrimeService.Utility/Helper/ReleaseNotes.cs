namespace PrimeService.Utility.Helper;

public class ReleaseNotes
{
    public List<Release> Release { get; set; }
}
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Release
{
    public string Version { get; set; }
    public string BuildVersion { get; set; }
    public string ReleaseDate { get; set; }
    public List<string> WhatsNew { get; set; }
    public List<string> Enhancement { get; set; }
    public List<string> Fixes { get; set; }
    public List<string> Next { get; set; }
}

