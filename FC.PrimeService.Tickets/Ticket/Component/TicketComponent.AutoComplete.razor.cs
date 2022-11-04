using Microsoft.AspNetCore.Components;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Utility.Helper;
using Model = PrimeService.Model.Tickets;
using Shop = PrimeService.Model.Shopping;
using Pay = PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Tickets.Ticket.Component;

public partial class TicketComponent
{

    #region ServiceType Search- Autocomplete
    private IEnumerable<ServiceType> _serviceTypes = new List<ServiceType>();
    private async Task<IEnumerable<ServiceType>> ServiceType_SearchAsync(string value)
    {
        var responseData = await Utilities.GetServiceTypes(_appSettings, _httpService, value);
        _serviceTypes = responseData.Items;
        Console.WriteLine($"Find Service Type : '{value}'" );
        CalculateAmount();
        return _serviceTypes;
    }


    #endregion
    
    #region Staff/Employee Search - Autocomplete

    private List<AuditUser> _employees = new List<AuditUser>();

    private async Task<IEnumerable<AuditUser>> Employee_SearchAsync(string value)
    {
        var responseData = await Utilities.GetEmployee(_appSettings, _httpService, value);
        var employees = responseData.Items;
        List<AuditUser> auditUserList = new List<AuditUser>();
        foreach (var emp in employees)
        {
            auditUserList.Add(emp.ToAuditUser(emp));
        }
        Utilities.ConsoleMessage($"Find Audit Users : '{value}'" );
        return auditUserList;
    }

    #endregion
    
    #region Client Search - Autocomplete

    private IEnumerable<Shop.Client> _clients = new List<Shop.Client>();
    async Task<IEnumerable<Shop.Client>> Client_SearchAsync(string value)
    {
        var responseData = await Utilities.GetClients(_appSettings, _httpService, value);
        _clients = responseData.Items;
        Console.WriteLine($"Find Payment Tags : '{value}'" );
        return _clients;
    }

    #endregion
    
    #region Sales Search - Autocomplete
    
    public List<Shop.Sales> _sales = new List<Shop.Sales>()
    {
        new Shop.Sales(){ BillNumber = "SL.2022.10"},
        new Shop.Sales(){ BillNumber = "SL.2022.11"},
        new Shop.Sales(){ BillNumber = "SL.2022.12"},
    };

    private Shop.Sales _selectedSales = new Shop.Sales();
    
    private async Task<IEnumerable<Shop.Sales>> Sales_SearchAsync(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);
        Console.WriteLine($"Find Sales : '{value}'");
        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
        {
            return _sales;
        }
        
        var result = _sales.Where
            (x => x.BillNumber.Contains(value, 
                StringComparison.InvariantCultureIgnoreCase));
        if (result.FirstOrDefault() == null)
        {
            result = new List<Shop.Sales>()
            {
                new Shop.Sales() { BillNumber = "Not Found" }
            };
        }
        else
        {
            _inputMode.BillNumbers.Add(result.FirstOrDefault().BillNumber);
        }
        return result;
    }
    
    #endregion
}