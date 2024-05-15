namespace TBotPlatform.Contracts.Bots.Pagination;

public class PaginationData<T>
    where T : class
{
    /// <summary>
    /// коллекция значений
    /// </summary>
    public List<T> Values { get; set; }

    /// <summary>
    /// Необходимость следующей страницы
    /// </summary>
    public bool IsNext { get; set; } = false;

    /// <summary>
    /// Необходимость предыдущей страницы
    /// </summary>
    public bool IsPrevious { get; set; } = false;

    /// <summary>
    /// Значение кнопки для перехоа на следующую страницу
    /// </summary>
    public string NextValue { get; set; }

    /// <summary>
    /// Значение кнопки для перехоа на предыдущую страницу
    /// </summary>
    public string PreviousValue { get; set; }
}