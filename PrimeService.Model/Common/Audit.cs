namespace PrimeService.Model.Common;

/// <summary>
/// A General Audit Control 
/// </summary>
public class Audit
{
    /// <summary>
    /// Model data created Date.
    /// </summary>
    public DateTime CreatedDate { get; set; }
    /// <summary>
    /// Recently modified date & time
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.Now;
}