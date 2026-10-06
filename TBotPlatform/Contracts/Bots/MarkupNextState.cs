using Newtonsoft.Json;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Extension;

namespace TBotPlatform.Contracts.Bots;

/// <summary>
/// Data from the inline button
/// </summary>
/// <param name="state">The state to invoke</param>
/// <param name="data">Data for the state</param>
public class MarkupNextState(string state, string? data = null)
{
    /// <summary>
    /// The state that needs to be invoked
    /// </summary>
    [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
    public string State { get; private set; } = state;

    /// <summary>
    /// Data for the state
    /// </summary>
    [JsonProperty("d", NullValueHandling = NullValueHandling.Ignore)]
    public string? Data { get; private set; } = data;

    public string[] GetDataWithoutDelimiter() => Data.IsNotNull() ? Data!.Split(DelimiterConstant.DelimiterFirst) : [];
}
