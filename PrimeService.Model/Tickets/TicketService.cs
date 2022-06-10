using System.Diagnostics;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Shopping;

namespace PrimeService.Model.Tickets;

/// <summary>
/// Ticket details were collected in this data.
/// </summary>
public class TicketService
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    public string? TicketNo { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    
    public Client Client { get; set; }
    public Employee EnteredBy { get; set; }
    public Employee AssignedTo { get; set; }
    public IList<ActivityTasks> TaskList { get; set; }

    #region Payments
    public double AdvanceAmount { get; set; }
    public double BalanceAmount { get; set; }
    public double TotalAmount { get; set; }
    
    /// <summary>
    /// A List of payments payed by customer/company.
    /// </summary>
    public IList<PaymentDetails>? Payments { get; set; }
    #endregion

    public string Appearance { get; set; }
    public string Reasons { get; set; }
    public DateTime? TargetDate { get; set; }
    public TicketType TicketType { get; set; }
    public Status TicketStatus { get; set; }

    public Dictionary<TicketType, Dictionary<string, string>> TicketTypeDetails
    {
        get;
        set;
    } = TicketProperty.GetDetails(TicketType.SmartPhone);
    
    public IList<ActivityLog> Activities { get; set; }

    /// <summary>
    /// Other details
    /// </summary>
    public Dictionary<string, string>? AdditionalDetails { get; set; } = new Dictionary<string, string>();
    /* {
         { "Key1", string.Empty },
         { "Key2", string.Empty },
         { "Key3", string.Empty }, 
     };*/

}

public enum TicketType
{
    GeneralService,
    GeneralRepair,
    Electronics,
    Bike,
    Mobile,
    SmartPhone,
    Appliances,
    Tablets,
    Laptop,
    PC,
    Furnitures,
    MusicInstruments
}


public class ActivityLog
{
    public DateTime ActivityDate { get; set; }
    public Employee ByWho { get; set; }
    public Employee AssignedFrom { get; set; }
    public Employee AssignedTo { get; set; }
    public string UserComments { get; set; }
    public Status FromStatus { get; set; }
    public Status ToStatus { get; set; }

    public string Log
    {
        get
        {
            string status = FromStatus == ToStatus ? FromStatus.ToString() + "=>" + ToStatus.ToString() : string
                .Empty;
    
            return string.Format("{0} {1} {2} {3}",
                ActivityDate.ToString("MMMM dd"),
                ActivityDate.ToString("h:mm tt"),
                status,
                ByWho.User.Name
            );
        }
    }
}


/// <summary>
/// Transaction Payment Details
/// </summary>
public class PaymentDetails
{
    /// <summary>
    /// A unique number generated for each payments
    /// </summary>
    public string? ReferenceNumber { get; set; }
    public PaymentTags? PaymentAccount { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public double PayedAmount { get; set; }
    public DateTime TransactionDate { get; set; }
   
}