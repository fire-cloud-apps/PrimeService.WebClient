using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings.Tickets;

/// <summary>
/// Ticket Status category eg. 'New', 'In Progress', 'Pending' ... etc 'Done'
/// </summary>
public class Status
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Tells which stage/order is 1, 2, 3 etc. This should be unique in nature. Stage1, used for a particular Status which cannot be assigned for another Status.
    /// Eg.New-1, InProgress-2, QC-3, PreCheck-4, Completion-5, Delivery-6 etc.
    /// </summary>
    public int Order
    {
        get;
        set;
    }
    
    /// <summary>
    /// Ticket Status Name
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }
    
    public string ColorCode { get; set; }
    //public StatusColor ColorCode { get; set; }
}

public enum StatusColor
{
    [Description("default")] Default,
    [Description("primary")] Primary,
    [Description("secondary")] Secondary,
    [Description("tertiary")] Tertiary,
    [Description("info")] Info,
    [Description("success")] Success,
    [Description("warning")] Warning,
    [Description("error")] Error,
    [Description("dark")] Dark,
    [Description("transparent")] Transparent,
    [Description("inherit")] Inherit,
    [Description("surface")] Surface,
}