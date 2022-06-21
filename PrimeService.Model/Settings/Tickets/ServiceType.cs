using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings.Tickets;

/// <summary>
/// List of Service provided by the Company
/// </summary>
public class ServiceType
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Service Title Name
    /// </summary>
    [Required(ErrorMessage = "Title Name is required.")]
    public string? Title { get; set; }

    /// <summary>
    /// Price of the service type
    /// </summary>
    public double Price { get; set; }
    /// <summary>
    /// Charges for the existing Service
    /// </summary>
    public double Cost { get; set; }
    /// <summary>
    /// Warranty can be applied or default Warranty will be applied.
    /// </summary>
    public int Warranty { get; set; }
    
    /// <summary>
    /// Service Category of the Service provided.
    /// </summary>
    public ServiceCategory? Category { get; set; }
}