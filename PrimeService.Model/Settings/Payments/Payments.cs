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
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;
    
    /// <summary>
    /// BillNo/SalesNo/Any associated no. to track, used to get, for which 'Sales', this 'Payment' is associated with.
    /// </summary>
    public string? BillNo { get; set; }

    public PaymentCategory PaymentCategory { get; set; }
    
    public DateTime TransactionDate { get; set; }
    
    public double IncomeAmount { get; set; }
    public double ExpenseAmount { get; set; }
    /// <summary>
    /// Purpose is only for temp transaction after that this will be categorized to 'Income' or 'Expense'. 
    /// </summary>
    public double Amount { get; set; }
    
    public string Reason { get; set; }
    
    public Employee Who { get; set; }

    public Client Client { get; set; } 

    #region Other
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentTags PaymentTag { get; set; }
    public PaymentMethods PaymentMethod { get; set; }

    #endregion
    
}

