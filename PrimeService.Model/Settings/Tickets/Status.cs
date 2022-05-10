using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings.Tickets;

/// <summary>
/// Ticket Status category
/// </summary>
public class Status
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Ticket Status Name
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }
}