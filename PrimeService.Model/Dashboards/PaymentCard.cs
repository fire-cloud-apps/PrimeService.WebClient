namespace PrimeService.Model.Dashboards;


public class PaymentCard
{
    /// <summary>
    /// Over all Account Balance
    /// </summary>
    public double OverAllBalance { get; set; }

    /// <summary>
    /// Total income to the Organization
    /// </summary>
    public double TotalRevenue { get; set; }
    
    /// <summary>
    /// Total expense/cost to the Organization
    /// </summary>
    public double TotalExpense { get; set; }

    private double _margin = 0.00d;
    /// <summary>
    /// Profit in Percentage called as 'Profit Margin'
    /// </summary>
    public double Margin 
    {
        get
        {
            return Math.Round(_margin, 2);
        }
        set
        {
            _margin = value;
        }
    }
    /// <summary>
    /// Over all profit to the organization
    /// </summary>
    public double Profit { get; set; }
}