using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Shopping;

namespace PrimeService.Model.Settings.Payments;

/// <summary>
/// Captures all the Money transaction for the organization.
/// </summary>
public class Payments
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    public PaymentCategory PaymentCategory { get; set; }
    
    public DateTime TransactionDate { get; set; }
    
    public double IncomeAmount { get; set; }
    public double ExpenseAmount { get; set; }
    
    public string Reason { get; set; }
    
    public Employee Who { get; set; }
    
    public Client Client { get; set; } 

    #region Other

    public PaymentTags PaymentTag { get; set; }
    public PaymentMethods PaymentMethod { get; set; }

    #endregion
    
}

