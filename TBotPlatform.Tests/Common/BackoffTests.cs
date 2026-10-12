using TBotPlatform.Common;

namespace TBotPlatform.Tests.Common;

/// <summary>
/// Tests the shared exponential-backoff-with-jitter helper used by the bot
/// startup supervisor and the update-polling retry loop.
/// </summary>
[TestFixture]
public class BackoffTests
{
    [Test]
    public void GetDelay_FirstAttempt_StaysWithinTheBaseInterval()
    {
        var delay = Backoff.GetDelay(1);

        Assert.Multiple(() =>
        {
            Assert.That(delay, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(delay, Is.LessThan(TimeSpan.FromSeconds(1)));
        });
    }

    [Test]
    public void GetDelay_RespectsTheBaseDelay()
    {
        for (var i = 0; i < 100; i++)
        {
            var delay = Backoff.GetDelay(1, TimeSpan.FromMilliseconds(250));

            Assert.That(delay, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(delay, Is.LessThan(TimeSpan.FromMilliseconds(250)));
        }
    }

    [Test]
    public void GetDelay_SecondAttempt_DoublesTheCeiling()
    {
        for (var i = 0; i < 100; i++)
        {
            var delay = Backoff.GetDelay(2, TimeSpan.FromMilliseconds(250));

            Assert.That(delay, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(delay, Is.LessThan(TimeSpan.FromMilliseconds(500)));
        }
    }

    [Test]
    public void GetDelay_AttemptZero_IsTreatedAsTheFirstAttempt()
    {
        var delay = Backoff.GetDelay(0, TimeSpan.FromMilliseconds(500));

        Assert.That(delay, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
        Assert.That(delay, Is.LessThan(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void GetDelay_VeryHighAttempt_IsCappedAtTheMaximumDelay()
    {
        for (var i = 0; i < 100; i++)
        {
            var delay = Backoff.GetDelay(1_000_000);

            Assert.That(delay, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(delay, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(30)));
        }
    }

    [Test]
    public void ResetThreshold_IsThirtySeconds()
        => Assert.That(Backoff.ResetThreshold, Is.EqualTo(TimeSpan.FromSeconds(30)));
}
