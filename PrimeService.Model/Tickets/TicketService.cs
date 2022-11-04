using System.ComponentModel;
using System.Diagnostics;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Shopping;
using PrimeService.Model.Utility;

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
    public AuditUser EnteredBy { get; set; }
    public AuditUser AssignedTo { get; set; }
    public IList<ActivityTasks> TaskList { get; set; } = new List<ActivityTasks>();
    public ServiceType ServiceType { get; set; }

    #region Payments
    public double PaidAmount { get; set; }
    public double AdvanceAmount { get; set; }
    public double BalanceAmount { get; set; }
    public double TotalAmount { get; set; }

    /// <summary>
    /// A List of payments payed by customer/company.
    /// </summary>
    public IList<Payments> Payments { get; set; } = new List<Payments>();
    #endregion

    public PaymentTags? PaymentAccount { get; set; }
    public DateTime? TargetDate { get; set; }
    public TicketType TicketType { get; set; }
    public Status TicketStatus { get; set; }
    
    #region Additional Details
    public string Appearance { get; set; }
    public string Reasons { get; set; } = string.Empty;
    public Dictionary<string, IList<CustomField>> TicketTypeDetails
    {
        get;
        set;
    } = TicketProperty.GetTicketCustomProperty(TicketType.SmartPhone);
    
    /// <summary>
    /// Other details
    /// </summary>
    public IList<CustomField>  AdditionalDetails { get; set; }
    /* {
         { "Key1", string.Empty },
         { "Key2", string.Empty },
         { "Key3", string.Empty }, 
     };*/
    #endregion
    
    /// <summary>
    /// Automatically created activities, by the user action.
    /// </summary>
    public IList<ActivityLog> Activities { get; set; } = new List<ActivityLog>();

    #region Planned for next Version
    /// <summary>
    /// Its an optional, 'BillNumber' from 'Sales', as a part of Ticket, if the customer purchases we can use this Id's. We should be able to associate one or more bills.
    /// </summary>
    public IList<string> BillNumbers { get; set; } //This should be in separate Tab
    #endregion
    
    /// <summary>
    /// Temporary comment which will capture and store in 'ActivityLog'
    /// </summary>
    public string UserLastComments { get; set; }
    
}

public enum TicketType
{
    [Description("All type of General Services")]
    GeneralService,
    [Description("All type of General Repairs")]
    GeneralRepair,
    [Description("All type of General Repairs")]
    Electronics,
    [Description("All type of General Repairs")]
    Bike,
    [Description("All type of General Repairs")]
    Mobile,
    [Description("All type of General Repairs")]
    SmartPhone,
    [Description("All type of General Repairs")]
    Appliances,
    [Description("All type of General Repairs")]
    Tablets,
    [Description("All type of General Repairs")]
    Laptop,
    [Description("All type of General Repairs")]
    PC,
    [Description("All type of General Repairs")]
    Furnitures,
    [Description("All type of Musical Instruments Service provided, with this group.")]
    MusicInstruments
}

public class ActivityLog
{
    /// <summary>
    /// A Unique SerialNo to handle 'Activity Logs'
    /// </summary>
    public int SerialNo { get; set; }
    public DateTime ActivityDate { get; set; }
    public AuditUser ByWho { get; set; }
    public AuditUser AssignedFrom { get; set; }
    public AuditUser AssignedTo { get; set; }
    /// <summary>
    /// Activity Notes.
    /// </summary>
    public string UserComments { get; set; }
    public Status FromStatus { get; set; }
    public Status ToStatus { get; set; }

    public string Log
    {
        get;
        set;
    } = string.Empty;
}
