namespace TBotPlatform.Contracts.Bots.States;

public class StateResult
{
    /// <summary>
    /// Признак необходимости отправки меню
    /// </summary>
    /// <returns></returns>
    public bool IsNeedUpdateMarkup { get; set; }

    /// <summary>
    /// Название следующего состояния
    /// </summary>
    public string NextStateName { get; set; }

    /// <summary>
    /// Дополнительная информация по состоянию
    /// </summary>
    public string Data { get; set; }
}