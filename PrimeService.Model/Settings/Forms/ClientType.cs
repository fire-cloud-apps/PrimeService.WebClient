using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings.Forms;

/// <summary>
/// Client Type, eg, 'Individual' or 'Company'
/// </summary>
public class ClientType
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
    [Required(ErrorMessage = "Client Type is required.")]
    public string? Title { get; set; }
}