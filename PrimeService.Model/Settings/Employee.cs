using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings;

/// <summary>
/// Service Employee 
/// </summary>
public class Employee
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    [Required]
    [StringLength(150, ErrorMessage = "Name length can't be more than 150.")]
    public string Name { get; set; } //Alternative of 'Name'
}