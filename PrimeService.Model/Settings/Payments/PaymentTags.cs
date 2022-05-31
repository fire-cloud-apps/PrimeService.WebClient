using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings.Payments;

/// <summary>
/// Payment Tag, used as Payment Account Categorization. To find in what category this payment is accounted for eg.'Service to Customer' or 'Expense of Refund'
/// </summary>
public class PaymentTags
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Payment Tag Name
    /// </summary>
    [Required(ErrorMessage = "Payment Tag Name is required.")]
    public string? Title { get; set; }
    
    /// <summary>
    /// Indicates that is the Income Type/Expense Type
    /// </summary>
    public PaymentCategory Category { get; set; }
    
    /// <summary>
    /// Initial fund for the account that has invested.
    /// </summary>
    public double InitialFund { get; set; }
    
}

