using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Location;

namespace PrimeService.Model.Settings;

/// <summary>
/// Account is also called as Organization
/// </summary>
public class Account
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    [Required]
    [StringLength(150, ErrorMessage = "Name length can't be more than 150.")]
    public string BusinessName { get; set; } //Alternative of 'Title'

    [Required]
    public string PrimaryMobile { get; set; }
    public string SecondaryMobile { get; set; }

    public string Address { get; set; }
    [Required]
    public string BusinessType { get; set; }

    [Required]
    public Country Country { get; set; }
    public string Currency { get; set; }

}