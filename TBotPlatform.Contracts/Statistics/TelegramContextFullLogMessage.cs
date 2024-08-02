﻿#nullable enable
using Newtonsoft.Json;

namespace TBotPlatform.Contracts.Statistics;

public class TelegramContextFullLogMessage
{
    public TelegramContextLogMessage? Request { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Result { get; set; }
}