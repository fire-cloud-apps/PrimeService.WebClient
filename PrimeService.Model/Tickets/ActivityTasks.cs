using PrimeService.Model.Common;
using PrimeService.Model.Settings;

namespace PrimeService.Model.Tickets;

public class ActivityTasks
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    // [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    // public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Generate a Serial No for unique identification of Tasks associated with 'TicketNo'
    /// </summary>
    public int SerialNo
    {
        get;
        set;
    }
    /// <summary>
    /// Just to associate to which Ticket this Task is linked to.
    /// </summary>
    public string TicketNo { get; set; }
        
    public string Title { get; set; }
    public string Notes { get; set; }

    public AuditUser AssignedTo { get; set; }
    public bool IsCompleted { get; set; }
    
    public DateTime? TargetDate { get; set; }
    
    public DateTime? CompletedDate { get; set; }
}

