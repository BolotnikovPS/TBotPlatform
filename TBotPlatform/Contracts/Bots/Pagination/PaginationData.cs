namespace TBotPlatform.Contracts.Bots.Pagination;

public class PaginationData<T>
    where T : class
{
    /// <summary>
    /// Collection of values
    /// </summary>
    public List<T> Values { get; set; } = null!;

    /// <summary>
    /// Whether the next page is needed
    /// </summary>
    public bool IsNext { get; set; } = false;

    /// <summary>
    /// Whether the previous page is needed
    /// </summary>
    public bool IsPrevious { get; set; } = false;

    /// <summary>
    /// Button value for navigating to the next page
    /// </summary>
    public string NextValue { get; set; } = null!;

    /// <summary>
    /// Button value for navigating to the previous page
    /// </summary>
    public string PreviousValue { get; set; } = null!;
}