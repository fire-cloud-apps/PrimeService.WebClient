using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Common;
using PrimeService.Model.Settings;

namespace PrimeService.Model.Shopping;

/// <summary>
/// Transaction on the given 'Product', which tells the stock 'In' & 'Out'.
/// </summary>
public class ProductTransaction
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;
    
    [Required]
    public string? ProductId { get; set; }
    
    [Required]
    public string? Reason { get; set; }
    [Required]
    public DateTime TransactionDate { get; set; }
    [Required]
    public AuditUser? Who { get; set; }
    public StockAction Action { get; set; }
    
    /// <summary>
    /// Quantity Involved during the transaction, if 'SOLD', the value will be in '-' if 'PURCHASED' value will be in '+'.
    /// </summary>
    [Required]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Transaction Amount involved.
    /// </summary>
    [Required]
    public double Price { get; set; }
}

public enum StockAction
{
    In,
    Out,
    Nill
}