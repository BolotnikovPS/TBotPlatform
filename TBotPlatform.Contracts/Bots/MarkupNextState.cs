using Newtonsoft.Json;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

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
    public string State { get; private set; } = state;

    /// <summary>
    /// Данные для состояния
    /// </summary>
    [JsonProperty("d", NullValueHandling = NullValueHandling.Ignore)]
    public string Data { get; private set; } = data;
    
    public string[] GetDataWithoutDelimiter()
    {
        return Data.CheckAny() ? Data.Split(DelimiterConstant.DelimiterFirst) : default;
    }
}