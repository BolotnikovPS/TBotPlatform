using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Cache;

public class UserStateInCache : IKeyInCache
{
    public List<string> StatesTypeName { get; set; } = null!;

    public string ChatId { get; set; } = null!;

    public string Key => ChatId;
}