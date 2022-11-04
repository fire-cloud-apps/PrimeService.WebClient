using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Humanizer;
using PrimeService.Model.Settings.Tickets;
using MudBlazor;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Shopping;

namespace PrimeService.Utility.Helper;

public static class Utilities
{
    /// <summary>
    /// Message to the client user.
    /// </summary>
    /// <param name="snackbar"></param>
    /// <param name="msg"></param>
    /// <param name="severity"></param>
    /// <param name="variant"></param>
    public static void SnackMessage(ISnackbar snackbar, string msg, 
        Severity severity = Severity.Success, Variant variant = Variant.Filled)
    {
        //Message
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
        snackbar.Configuration.SnackbarVariant = variant;
        //Snackbar.Configuration.VisibleStateDuration  = 2000;
        //Can also be done as global configuration. Ref:
        //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
        snackbar.Add(msg, severity);
    }

    /// <summary>
    /// Delete Confirmation Message Box
    /// </summary>
    /// <param name="DialogService">Injection Dialog Service</param>
    /// <returns>Can Delete or not 'bool' value.</returns>
    public static async Task<bool> DeleteConfirm(IDialogService DialogService, string title = "Warning", string message = "Heads Up! Are you sure?. Deleting can not be undone!",
        string yesTxt ="Delete!")
    {
        bool? result = await DialogService.ShowMessageBox(
            title,
            message, 
            yesText:yesTxt, cancelText:"Cancel");
        var canDelete = result == null ? false : true;
        Console.WriteLine( $"Can Delete : {canDelete}");
        return canDelete;
    }

    /// <summary>
    /// Write to console message if 'DEBUG' Mode
    /// </summary>
    /// <param name="msg">Message to Write in console</param>
    public static void ConsoleMessage(string msg)
    {
        #if DEBUG
        Console.WriteLine(msg);
        #endif
    }

    /// <summary>
    /// Return only the string to the given length eg. "Dialog box will be opened..."
    /// </summary>
    /// <param name="value">value to split - string value</param>
    /// <param name="suffix">eg. "..." or ".." or "-" etc. Default '...' </param>
    /// <param name="length">length of the string to return. Default '30'</param>
    /// <returns></returns>
    public static string GetCharterByLength(string value, string suffix = "...", short length = 30)
    {
        string firstGivenChar = 
            !String.IsNullOrWhiteSpace(value) && value.Length >= length
                ? value.Substring(0, length) + suffix
                : value;
        return firstGivenChar;
    }
    
    /// <summary>
    /// Parse JWT Token
    /// </summary>
    /// <param name="jwt">JWT Token String</param>
    /// <returns>returns Claims</returns>
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
    
    public static async Task<PaymentTags> GetDefault_PaymentAccount(AppSettings appSettings, IHttpService httpService)
    {
        string url = string.Empty;
        url = $"{appSettings.App.ServiceUrl}{appSettings.API.PaymentTagsApi.GetDefault}";
        Utilities.ConsoleMessage($"Default PaymentTag : {url}");
        var accounts = await httpService.GET<PaymentTags>(url);
        
        return accounts;
    }
    
    public static async Task<PaymentMethods> GetDefault_PaymentMethods(AppSettings appSettings, IHttpService httpService)
    {
        string url = string.Empty;
        url = $"{appSettings.App.ServiceUrl}{appSettings.API.PaymentMethodsApi.GetDefault}";
        Utilities.ConsoleMessage($"Default PaymentMethods: {url}");
        var accounts = await httpService.GET<PaymentMethods>(url);
        
        return accounts;
    }
    public static async Task<ResponseData<Client>> GetClients(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Mobile")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.ClientApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<Client>>(url, pageMetaData);
        return responseModel;
    }
    
    public static async Task<ResponseData<ClientType>> GetClientType(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Title")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.ClientTypeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<ClientType>>(url, pageMetaData);
        return responseModel;
    }
    
    public static async Task<ResponseData<Status>> GetTicketStatus(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Title")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.TicketStatusApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<Status>>(url, pageMetaData);
        return responseModel;
    }
    
    public static async Task<ResponseData<ServiceType>> GetServiceTypes(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Title")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.ServiceTypeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<ServiceType>>(url, pageMetaData);
        return responseModel;
    }
    
    public static async Task<ResponseData<Employee>> GetEmployee(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Title")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.EmployeeApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<Employee>>(url, pageMetaData);
        return responseModel;
    }
    
    public static async Task<ResponseData<PaymentTags>> GetPaymentTags(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Title")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.PaymentTagsApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<PaymentTags>>(url, pageMetaData);
        return responseModel;
    }
    
    public static async Task<ResponseData<PaymentMethods>> GetPaymentMethods(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Title")
    {
        string url = $"{appSettings.App.ServiceUrl}{appSettings.API.PaymentMethodsApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Title",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<PaymentMethods>>(url, pageMetaData);
        return responseModel;
    }

    public static async Task<AuditUser> GetLoginUser(ILocalStorageService localStore)
    {
        if (GlobalConfig.LoginUser == null)
        {
            GlobalConfig.LoginUser = await localStore.GetItemAsync<AuditUser>("LoginUser");
        }
        return GlobalConfig.LoginUser;

    }
    
    public static string CurrencyConvert(double value)
    {
        //  Input           =>  Output
        //  25000           =>  25K
        //  125000          =>  1.25L
        //  12,55,000       =>  12.55L
        //  1,35,25,000     =>  1.35C
        //  25,75,62,000    =>  25.75C
        
        string returnString = string.Empty;
        double oneLakh = 100000d;
        double returnValue = 0d;
        double numOfLakh = value / oneLakh;
        
        if (numOfLakh > 0)
        {
            if (numOfLakh >= 100)
            {
                returnValue = numOfLakh / 100;
                returnString = "C".ToQuantity(returnValue, "C2", new CultureInfo("en-IN")) ;
            }
            else
            {
                returnValue = numOfLakh;
                returnString = "L".ToQuantity(returnValue, "C2", new CultureInfo("en-IN")) ;
            }
        }
        else
        {
            returnValue = numOfLakh * 100;
            returnString = "K".ToQuantity(returnValue, "C2", new CultureInfo("en-IN")) ;
        }

        returnString = returnString.Replace("INR", "₹");
        ConsoleMessage($"Source Value: {value} Before Conversion: {numOfLakh} Return Value: {returnString}");
        return returnString;
    }
    
    /// <summary>
    /// Get the enum description.
    /// Eg. int value = 1;
    ///string description = Utilities.GetEnumDescription((MyEnum)value);
    /// </summary>
    /// <param name="value">some integer value</param>
    /// <returns>returns description</returns>
    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        
        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }
        return value.ToString();
    }
}