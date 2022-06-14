namespace PrimeService.Utility.Helper;


public class PageMetaData
{
    /// <summary>
    /// Server Side Search for the fixed fields.
    /// </summary>
    public string SearchText { get; set; }
    /// <summary>
    /// What is the page index, 1, or 2 or 3 etc.
    /// which usually called as how many page to skip
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Total records to be displayed per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Field Name to sort
    /// </summary>
    public string SortLabel { get; set; }
    
    /// <summary>
    /// If not provided it will use the default search field.
    /// </summary>
    public string SearchField { get; set; }

    /// <summary>
    /// A - Ascending, D - Descending
    /// </summary>
    public string SortDirection { get; set; } = "A";

}