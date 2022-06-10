using PrimeService.Model.Shopping;

namespace PrimeService.Utility.Helper;

public class AppSettings
{
    public App App { get; set; }
    /// <summary>
    /// Application Version to be displayed in the Main title.
    /// </summary>
    public string? Version { get; set; }
    public string? Build { get; set; }
    public bool IsDev { get; set; }
    
    public API API { get; set; }
   
}


public class App
{
    public string AuthUrl { get; set; }
    public string GoogleAuth { get; set; }
    public string ServiceUrl { get; set; }
}

public class API
{
     public CompanyApi CompanyApi { get; set; }
}

public class CompanyApi
{
 public string GetDetails { get; set; }
// "GetBatch":"",
// "Create":"",
// "Update":"",
// "Delete":""
}