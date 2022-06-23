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
     public TicketStatusApi TicketStatusApi { get; set; }
     public SequenceApi SequenceApi { get; set; }
     public LicenseSubscriptionApi LicenseSubscriptionApi { get; set; }
     public WorkLocationApi WorkLocationApi { get; set; }
     public EmployeeApi EmployeeApi { get; set; }
     
}

#region Employee API

public class EmployeeApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion

#region Work Location API

public class WorkLocationApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion

#region Company API
public class CompanyApi
{
 public string GetDetails { get; set; }
// "GetBatch":"",
// "Create":"",
// "Update":"",
// "Delete":""
}
#endregion

public class LicenseSubscriptionApi
{
    public string GetSubscription { get; set; }
    public string GetAccount { get; set; }
}

public class SequenceApi
{
    public string GetDetails { get; set; }
    public string Generate { get; set; }
}

#region Ticket Status API

public class TicketStatusApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion
