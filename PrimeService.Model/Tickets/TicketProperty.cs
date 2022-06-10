using System.Runtime.CompilerServices;
using PrimeService.Model.Settings.Tickets;

namespace PrimeService.Model.Tickets;

/// <summary>
/// Load the fields based on the 'Ticket Type'
/// </summary>
public class TicketProperty
{
    
    public static Dictionary<TicketType, Dictionary<string, string>> GetDetails(TicketType type)
    {
        Dictionary<TicketType, Dictionary<string, string>> returnData = null;
        switch (type)
        {
            case TicketType.Appliances:
                returnData = Appliances;
                break;
            case TicketType.Mobile:
                returnData = SmartPhone;
                break;
            case TicketType.Bike:
                returnData = Bike;
                break;
            default:
                returnData = SmartPhone;
                break;
        }

        return returnData;
    }
    public static Dictionary<TicketType, Dictionary<string, string>> Bike = new Dictionary<TicketType, Dictionary<string, string>>()
    {
        {
            TicketType.SmartPhone, new Dictionary<string, string>()
            {
                {"Registration No", string.Empty},
                {"Batch Id", string.Empty},
                {"Model", string.Empty},
                {"Fuel Type", string.Empty},
                {"Serial No", string.Empty}
            }
        }
    };
    public static Dictionary<TicketType, Dictionary<string, string>> SmartPhone = new Dictionary<TicketType, Dictionary<string, string>>()
    {
        {
            TicketType.SmartPhone, new Dictionary<string, string>()
            {
                {"IMEI No", string.Empty},
                {"Serial No", string.Empty},
                {"Model", string.Empty},
                {"Equipments", string.Empty},
                {"Device Password", string.Empty}
            }
        }
    };
    
    public static Dictionary<TicketType, Dictionary<string, string>> Appliances = new Dictionary<TicketType, Dictionary<string, string>>()
    {
        {
            TicketType.Appliances, new Dictionary<string, string>()
            {
                {"Batch No", string.Empty},
                {"Serial No", string.Empty},
                {"Model", string.Empty},
                {"Warranty", string.Empty},
                {"Company Code", string.Empty}
            }
        }
    };

    
}