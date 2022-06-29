using PrimeService.Model.Shopping;

namespace PrimeService.Utility.Helper;

public class App
{
    public string AuthUrl { get; set; }
    public string GoogleAuth { get; set; }
    public string ServiceUrl { get; set; }
}

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




public class API
{
     public CompanyApi CompanyApi { get; set; }
     public TicketStatusApi TicketStatusApi { get; set; }
     public SequenceApi SequenceApi { get; set; }
     public LicenseSubscriptionApi LicenseSubscriptionApi { get; set; }
     public WorkLocationApi WorkLocationApi { get; set; }
     public EmployeeApi EmployeeApi { get; set; }
     
     public TicketDefaultApi TicketDefaultApi { get; set; }
     
     public ServiceCategoryApi ServiceCategoryApi { get; set; }
     
     public ServiceTypeApi ServiceTypeApi { get; set; }
     
     public PaymentMethodsApi PaymentMethodsApi { get; set; }
     
     public PaymentTagsApi PaymentTagsApi { get; set; }
     
     public ClientTypeApi ClientTypeApi { get; set; }
     public ProductCategoryApi ProductCategoryApi { get; set; }
     public TaxApi TaxApi { get; set; }
     public ClientApi ClientApi { get; set; }
     public ProductApi ProductApi { get; set; }
     //
     public ProductTransactionApi ProductTransactionApi { get; set; }
}

#region ProductTransactionApi API
public class ProductTransactionApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}
#endregion

#region ProductApi API
public class ProductApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}
#endregion

#region ClientApi API
public class ClientApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}
#endregion

#region TaxApi API
public class TaxApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}
#endregion

#region ProductCategoryApi API
public class ProductCategoryApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}
#endregion

#region ClientTypeApi API
public class ClientTypeApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}
#endregion

#region PaymentTagsApi API

public class PaymentTagsApi
{
    public string GetDetails { get; set; }

    public string SetDefault {
        get;
        set;
    }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion

#region PaymentMethodsApi API

public class PaymentMethodsApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion

#region ServiceTypeApi API

public class ServiceTypeApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion

#region ServiceCategoryApi API

public class ServiceCategoryApi
{
    public string GetDetails { get; set; }
    public string GetBatch { get; set; }
    public string Fake { get; set; }
    public string Create { get; set; }
    public string Update { get; set; }
    public string Delete { get; set; }
}

#endregion

#region Employee API

public class EmployeeApi
{
    public string GetByEmail { get; set; }
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

#region More
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
#endregion

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

#region Ticket Default Settings

public class TicketDefaultApi
{
    public string GetDefault { get; set; }
    public string AddOrUpdate { get; set; }
}

#endregion