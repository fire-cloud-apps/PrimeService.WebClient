using PrimeService.Model.Common;

namespace PrimeService.Utility.Helper;

public class GlobalConfig
{
    public static AppSettings? AppSettings
    {
        get;
        set;
    }

    public static AuditUser LoginUser
    {
        get;
        set;
    }
    
    
}