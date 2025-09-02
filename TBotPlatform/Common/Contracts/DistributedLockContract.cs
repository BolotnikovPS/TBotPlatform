using TBotPlatform.Contracts.Abstractions.Cache;

namespace TBotPlatform.Common.Contracts;

internal class DistributedLockContract : IKeyInCache
{
    public string Key { get; set; }
    public DateTime Value { get; set; }
}