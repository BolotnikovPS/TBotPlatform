namespace TBotPlatform.Common;

/// <summary>
/// Exponential backoff with full jitter, shared by the bot startup and the update-polling retry loops.
/// </summary>
internal static class Backoff
{
    private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(30);

    /// <summary>
    /// After this quiet period without polling errors the consecutive-error counter is reset,
    /// so an isolated outage after a long healthy period starts from the first attempt again.
    /// </summary>
    public static TimeSpan ResetThreshold { get; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Returns a random delay in [0, min(base * 2^(attempt-1), 30 s)] (full jitter, AWS-recommended shape).
    /// </summary>
    /// <param name="attempt">1-based attempt counter.</param>
    /// <param name="baseDelay">Base interval for the first retry. Defaults to 1 second.</param>
    public static TimeSpan GetDelay(int attempt, TimeSpan? baseDelay = null)
    {
        var baseMs = Math.Max(1L, (long)(baseDelay ?? TimeSpan.FromSeconds(1)).TotalMilliseconds);
        var shift = Math.Clamp(attempt - 1, 0, 5);
        var ceilingMs = Math.Min(baseMs << shift, (long)MaxDelay.TotalMilliseconds);

        return TimeSpan.FromMilliseconds(Random.Shared.NextInt64(ceilingMs));
    }
}
