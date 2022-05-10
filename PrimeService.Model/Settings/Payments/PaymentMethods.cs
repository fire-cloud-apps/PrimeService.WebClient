using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings.Payments;

/// <summary>
/// Supported Payment methods such as 'Cash', 'Card', 'QR' etc.
/// </summary>
public class PaymentMethods
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Payment Method
    /// </summary>
    [Required(ErrorMessage = "Payment Method is required.")]
    public string? Title { get; set; }
}