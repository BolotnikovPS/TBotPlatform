using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Contracts.Cache.Lock;

internal class DistributedLockContract : IKeyInCache
{
    public string Key { get; set; } = "";

    public Guid OwnerId { get; set; }

    public DateTime Value { get; set; }
}
