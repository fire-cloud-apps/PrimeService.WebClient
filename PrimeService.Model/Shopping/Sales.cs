using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Common;
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
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Auto Generated Bill Number
    /// </summary>
    public string? BillNumber { get; set; }
    
    /// <summary>
    /// Status (Draft, Void, OnHold, Closed, Confirmed)
    /// </summary>
    public SalesStatus Status { get; set; }
    
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
    public AuditUser? BilledBy { get; set; } 
    
    /// <summary>
    /// Total Quantity purchased.
    /// </summary>
    public int TotalQuantity { get; set; }
    public Client Client { get; set; }
    /// <summary>
    /// Some General Description of the Sales if any.
    /// </summary>
    public string? Notes { get; set; } = string.Empty;
    
    #region Product
    
    public IList<PurchasedProduct>? Products { get; set; }
    
    #endregion

    #region Payments
    public double AdditonalCost { get; set; }
    public double TotalTax { get; set; }
    /// <summary>
    /// Total Discount in Amount/Rupees
    /// </summary>
    public double TotalDiscount { get; set; }
    public double SubTotal { get; set; }
    /// <summary>
    /// The complete total amount as a sales transaction
    /// </summary>
    public double GrandTotal { get; set; }
    /// <summary>
    /// Additional cost if any. Eg for Gift wrapping or other services.
    /// </summary>
    public PaymentTags? PaymentAccount { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    
    #endregion
    
}

/// <summary>
/// Status of Sales
/// </summary>
public enum SalesStatus
{
    /// <summary>
    /// Draft - This indicates that a SO has been created successfully, but is yet to be sent to the customer.
    /// </summary>
    [Description("Draft - This indicates that a SO has been created successfully, but is yet to be sent to the customer.")]
    Draft,
    /// <summary>
    /// Confirmed - This indicates that the SO created has been sent to the customer.
    /// </summary>
   [Description("Confirmed - This indicates that the SO created has been sent to the customer.")]
    Confirmed,
    /// <summary>
    /// Closed - The SO becomes Closed when you either raise an Invoice or when a Shipment is fulfilled (or both, depending on what you’ve chosen in the Sales Order Preferences.)
    /// </summary>
    [Description("Closed - The SO becomes Closed when you either raise an Invoice or when a Shipment is fulfilled (or both, depending on what you’ve chosen in the Sales Order Preferences.)")]
    Closed,
    /// <summary>
    /// Void/Cancelled - The SO status becomes Void, when you decide to freeze/nullify the SO and make it void.
    /// </summary>
    [Description("Void/Cancelled - The SO status becomes Void, when you decide to freeze/nullify the SO and make it void.")]
    Void,
    /// <summary>
    /// On Hold - The status is set as On Hold, when there’s an un-billed back-ordered PO raised for the Sales Order. Once the PO has been billed, the SO will revert back to its previous status.
    /// </summary>
    [Description("On Hold - The status is set as On Hold, when there’s an un-billed back-ordered PO raised for the Sales Order. Once the PO has been billed, the SO will revert back to its previous status.")]
    OnHold,
}

public enum PaymentStatus
{
    [Description("Complete Payment Done")]
    Paid,
    [Description("Partialy Payed by the customer")]
    PartiallyPaid,
    [Description("Yet to pay")]
    Pending,
    [Description("Amount refunded to customer")]
    Refund,
    [Description("Cancelled - due to customer action.")]
    Cancelled,
    [Description("Failed - due to some technical issue.")]
    Failed 
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
    public double Discount { get; set; }
    /// <summary>
    /// Discount, price calculation
    /// </summary>
    public double DiscountPrice { get; set; }
    public Tax? AppliedTax { get; set; }
}