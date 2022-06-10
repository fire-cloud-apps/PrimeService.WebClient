using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Common;
using PrimeService.Model.Settings.Forms;

namespace PrimeService.Model.Shopping;

/// <summary>
/// Clients of the Prime Service
/// </summary>
public class Client
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Client Name
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Name length can't be more than 150.")]
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// Client Mobile Number.
    /// </summary>
    [Required]
    [StringLength(12, ErrorMessage = "Mobile length can't be more than 12.")]
    public string? Mobile { get; set; } = string.Empty;
    
    /// <summary>
    /// Client Type, Individual/Company
    /// </summary>
    [Required]
    public ClientType? Type { get; set; }

    /// <summary>
    /// A General notes about the client.
    /// </summary>
    public string Note { get; set; } = string.Empty;
}