using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Cache;

public class UserBindStateInCache : IKeyInCache
{
    public string StatesTypeName { get; set; } = null!;

    public string ChatId { get; set; } = null!;

    public string Key => ChatId;
}