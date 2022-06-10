using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Shopping;

/// <summary>
/// Product Category used in the inventory to categorize the product.
/// </summary>
public class ProductCategory
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Service Category Name
    /// </summary>
    [Required(ErrorMessage = "Category Name is required.")]
    public string? CategoryName { get; set; }
}