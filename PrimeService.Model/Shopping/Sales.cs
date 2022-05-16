using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Payments;

namespace PrimeService.Model.Shopping;

/// <summary>
/// Used to manage Sales Data
/// </summary>
public class Sales
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Auto Generated Bill Number
    /// </summary>
    public string? BillNumber { get; set; }
    
    /// <summary>
    /// Sales Tag name for ease of identify , can be auto generated.
    /// </summary>
    public string? SalesTag { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }
    /// <summary>
    /// Employee of the Organization, who handled the sales.
    /// </summary>
    [Required]
    public Employee? BilledBy { get; set; } 
    
    /// <summary>
    /// Total Quantity purchased.
    /// </summary>
    public int Quantity { get; set; }
    public Client Client { get; set; }
    /// <summary>
    /// Some General Description of the Sales if any
    /// </summary>
    public string? Notes { get; set; } = string.Empty;
    #region Product
    public double Total { get; set; }
    public double TotalDiscount { get; set; }
    public IList<PurchasedProduct>? Products { get; set; }
    
    #endregion

    #region Payments
    /// <summary>
    /// Additional cost if any. Eg for Gift wrapping or other services.
    /// </summary>
    public double AdditonalCost { get; set; }
    public PaymentTags? PaymentAccount { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public double TotalTax { get; set; }
    #endregion
    
}

public enum PaymentStatus
{
    Paid,
    PartiallyPaid,
    Pending,
    Refund,
    Cancelled, // Cancelled - due to customer action.
    Failed // Failed - due to some technical issue.
}

public class PurchasedProduct
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double SubTotal { get; set; }
    /// <summary>
    /// This percentage value, comes from the Product itself.
    /// </summary>
    public int Discount { get; set; }
    
    public Tax? AppliedTax { get; set; }
    
}