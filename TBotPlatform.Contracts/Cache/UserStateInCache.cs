using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Cache;

public class UserStateInCache : IKeyInCache
{
    public List<string> StatesTypeName { get; set; }

    public string ChatId { get; set; }

    public string Key => ChatId;
}