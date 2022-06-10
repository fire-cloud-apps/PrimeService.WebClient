using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings;

public class Tax
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Tax Service name
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Rate of Interest applied for a product.
    /// </summary>
    public float TaxRate { get; set; }

    
    
    // <summary>
    /// Tax Description
    /// </summary>
    public string? Description { get; set; }
}

