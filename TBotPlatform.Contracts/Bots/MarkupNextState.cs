using Newtonsoft.Json;

namespace TBotPlatform.Contracts.Bots;

/// <summary>
/// Данные которые будут передаваться по нажатию кнопки inline
/// </summary>
/// <param name="state"></param>
/// <param name="data"></param>
public class MarkupNextState(string state, string data = null)
{
    /// <summary>
    /// Состояние которое необходимо вызывать
    /// </summary>
    [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
    public string State { get; set; } = state;

    /// <summary>
    /// Данные для состояния
    /// </summary>
    [JsonProperty("d", NullValueHandling = NullValueHandling.Ignore)]
    public string Data { get; set; } = data;
}