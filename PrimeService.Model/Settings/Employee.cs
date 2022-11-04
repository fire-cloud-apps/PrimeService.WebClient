using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Common;

namespace PrimeService.Model.Settings;

/// <summary>
/// Service Employee
/// </summary>
public class Employee
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Activated User
    /// </summary>
    public User User { get; set; }
    //[MaxLength(10, ErrorMessage = "Salary length can't be more than 10.")]
    public double Salary { get; set; }
    /// <summary>
    /// Current Work Location
    /// </summary>
    public WorkLocation WorkLocation { get; set; }
    [StringLength(15, ErrorMessage = "Mobile length can't be more than 15.")]
    public string Mobile { get; set; }
    
    public AuditUser ToAuditUser(Employee emp)
    {
        return new AuditUser()
        {
            AccountId = emp.User.AccountId,
            Name = emp.User.Name,
            EmailId = emp.User.Email,
            Picture = emp.User.Picture,
            UserId = emp.Id
        };
    }
}

