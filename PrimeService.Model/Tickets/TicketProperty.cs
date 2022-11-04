using System.Runtime.CompilerServices;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Utility;

namespace PrimeService.Model.Tickets;

/// <summary>
/// Load the fields based on the 'Ticket Type'
/// </summary>
public class TicketProperty
{
    public static Dictionary<string, IList<CustomField>> GetTicketCustomProperty(TicketType type)
    {
        Dictionary<string, IList<CustomField>> returnData = null;
        switch (type)
        {
            case TicketType.Appliances:
                returnData = BikeFields;
                break;
            case TicketType.Mobile:
                returnData = BikeFields;
                break;
            case TicketType.Bike:
                returnData = BikeFields;
                break;
            default:
                returnData = BikeFields;
                break;
        }

        return returnData;
    }

    #region Custom Field Value

    private static Dictionary<string, IList<CustomField>> BikeFields = new Dictionary<string, IList<CustomField>>()
    {
        {
            TicketType.Bike.ToString(), new List< CustomField>()
            {
                new CustomField()
                {
                    Property = "Registration No",
                    Value = ""
                },
                new CustomField()
                {
                    Property = "Batch Id",
                    Value = ""
                },
                new CustomField()
                {
                    Property = "Model",
                    Value = ""
                },
                new CustomField()
                {
                    Property = "Fuel Type",
                    Value = ""
                },
                new CustomField()
                {
                    Property = "Serial No",
                    Value = ""
                }
            }
        }
    };

    #endregion
    public static Dictionary<string, Dictionary<string, string>> GetTicketProperty(TicketType type)
    {
        Dictionary<string, Dictionary<string, string>> returnData = null;
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
    
    
    public static Dictionary<string, Dictionary<string, string>> Bike = new Dictionary<string, Dictionary<string, string>>()
    {
        {
            TicketType.Bike.ToString(), new Dictionary<string, string>()
            {
                {"Registration No", string.Empty},
                {"Batch Id", string.Empty},
                {"Model", string.Empty},
                {"Fuel Type", string.Empty},
                {"Serial No", string.Empty}
            }
        }
    };
    public static Dictionary<string, Dictionary<string, string>> SmartPhone = new Dictionary<string, Dictionary<string, string>>()
    {
        {
            TicketType.SmartPhone.ToString(), new Dictionary<string, string>()
            {
                {"IMEI No", string.Empty},
                {"Serial No", string.Empty},
                {"Model", string.Empty},
                {"Equipments", string.Empty},
                {"Device Password", string.Empty}
            }
        }
    };
    
    public static Dictionary<string, Dictionary<string, string>> Appliances = new Dictionary<string, Dictionary<string, string>>()
    {
        {
            TicketType.Appliances.ToString(), new Dictionary<string, string>()
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

