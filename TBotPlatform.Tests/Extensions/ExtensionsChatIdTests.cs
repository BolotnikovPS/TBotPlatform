using NUnit.Framework;
using TBotPlatform.Extension;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsChatIdTests
{
    [Test]
    public void ThrowIfInvalidChatId_WhenValid_DoesNotThrow()
        => Assert.DoesNotThrow(() => 123L.ThrowIfInvalidChatId());

    [TestCase(0)]
    [TestCase(long.MinValue)]
    [TestCase(long.MaxValue)]
    public void ThrowIfInvalidChatId_WhenInvalid_Throws(long chatId)
        => Assert.Throws<ArgumentOutOfRangeException>(() => chatId.ThrowIfInvalidChatId());
}
