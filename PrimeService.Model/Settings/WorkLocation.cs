using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Settings;

public class WorkLocation
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    public string Title { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}