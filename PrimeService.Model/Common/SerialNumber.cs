using System.Net.Sockets;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Common;

public class SerialNumber
{
    /// <summary>
    /// A Unique Id
    /// </summary>
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string TableName { get; set; } = string.Empty;
    public string Suffix { get; set; }= string.Empty;
    public string Prefix { get; set; }= string.Empty;
    public int SerialNo { get; set; }
    /// <summary>
    /// Batch no mostly it should be current 'Year' or 'Month' or 'Day'
    /// </summary>
    public int BatchNo { get; set; }

    public string Separator { get; set; } = ".";

    public string Preview
    {
        get
        {
            string tSep = Separator;
            if (string.IsNullOrEmpty(tSep))
            {
                tSep = ".";
            }
            return $"{Prefix}{tSep}{BatchNo}{tSep}{SerialNo}{tSep}{Suffix}".Trim(
                tSep.ToCharArray()[0]);
        }
    }

    
}