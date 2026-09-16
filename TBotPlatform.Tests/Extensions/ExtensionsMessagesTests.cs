using NUnit.Framework;
using TBotPlatform.Common;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsMessagesTests
{
    [Test]
    public void WithDocument_NullMessage_ReturnsFalse()
        => Assert.That(((Message?)null).WithDocument(), Is.False);

    [Test]
    public void WithDocument_ImageDocument_ReturnsFalse()
        => Assert.That(new Message { Document = new Document { MimeType = "image/png" } }.WithDocument(), Is.False);

    [Test]
    public void WithDocument_NonImageDocument_ReturnsTrue()
        => Assert.That(new Message { Document = new Document { MimeType = "application/pdf" } }.WithDocument(), Is.True);

    [Test]
    public void WithImage_NullMessage_ReturnsFalse()
        => Assert.That(((Message?)null).WithImage(), Is.False);

    [Test]
    public void WithImage_ImageDocument_ReturnsTrue()
        => Assert.That(new Message { Document = new Document { MimeType = "image/png" } }.WithImage(), Is.True);

    [Test]
    public void WithImage_NonImageDocument_ReturnsFalse()
        => Assert.That(new Message { Document = new Document { MimeType = "application/pdf" } }.WithImage(), Is.False);

    [Test]
    public void TryGetText_NullMessage_ReturnsFalse()
    {
        var ok = ((Message?)null).TryGetText(out var text);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(text, Is.Null);
        }
    }

    [Test]
    public void TryGetText_TextMessage_ReturnsText()
    {
        var ok = new Message { Text = "hello" }.TryGetText(out var text);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(text, Is.EqualTo("hello"));
        }
    }

    [Test]
    public void TryGetText_ImageDocument_ReturnsCaption()
    {
        var ok = new Message { Caption = "caption", Document = new Document { MimeType = "image/png" } }.TryGetText(out var text);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(text, Is.EqualTo("caption"));
        }
    }

    [Test]
    public void TryGetText_NullTextMessage_ReturnsFalse()
        => Assert.That(new Message { Text = null }.TryGetText(out _), Is.False);
}
