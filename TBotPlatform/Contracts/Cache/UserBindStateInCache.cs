using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Cache;

public class UserBindStateInCache : IKeyInCache
{
    public string StatesTypeName { get; set; }

    public string ChatId { get; set; }

    public string Key => ChatId;
}