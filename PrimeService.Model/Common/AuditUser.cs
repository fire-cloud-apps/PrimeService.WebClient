using PrimeService.Model.Settings;

namespace PrimeService.Model.Common;

/// <summary>
/// To Capture Transaction details
/// </summary>
public class AuditUser
{
    /// <summary>
    /// User Unique Id
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// User Email Id
    /// </summary>
    public string? EmailId { get; set; }
    /// <summary>
    /// User Name
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Associated Account
    /// </summary>
    public string? AccountId { get; set; }
    /// <summary>
    /// User Picture in URL or in Base64
    /// </summary>
    public string? Picture { get; set; }

    
}