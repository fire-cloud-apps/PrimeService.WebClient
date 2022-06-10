using System.ComponentModel.DataAnnotations;

namespace PrimeService.Model.Settings.Tickets;

/// <summary>
/// A Common Default settings on 'Service', 'Ticket' & 'Common'
/// </summary>
public class DefaultSettings
{
    /// <summary>
    /// Default Deadline provided by the organization
    /// </summary>
    [Required(ErrorMessage = "Deadline is required.")]
    public int Deadline { get; set; } = 3;
    /// <summary>
    /// Default Warranty provided by the organization 
    /// </summary>
    [Required(ErrorMessage = "Warranty is required.")]
    public int Warranty { get; set; } = 30;
}