using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace FireCloud.WebClient.PrimeService.Helper;

public static class QueryStringExtension
{
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    public static string QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key];
    }
}