using PrimeService.Model.Settings;

namespace PrimeService.Model.Tickets;

public class ActivityTasks
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    public string? Id { get; set; } = string.Empty;
    
    public string Title { get; set; }
    public string Notes { get; set; }

    public Employee AssignedTo { get; set; }
    public bool IsCompleted { get; set; }
    
    public DateTime? TargetDate { get; set; }
    
    public DateTime CompletedDate { get; set; }
}

