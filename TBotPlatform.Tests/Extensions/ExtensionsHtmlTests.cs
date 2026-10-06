using NUnit.Framework;
using TBotPlatform.Extension;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsHtmlTests
{
    [Test]
    public void ToEscapeHtml_WhenTextHasSpecialChars_ReplacesThem()
    {
        var result = "a & <b> \"c\"".ToEscapeHtml();

        Assert.That(result, Is.EqualTo("a &amp; &lt;b&gt; &quot;c&quot;"));
    }

    [Test]
    public void ToEscapeHtml_WhenTextIsNull_ReturnsNull()
    {
        string? text = null;

        Assert.That(text.ToEscapeHtml(), Is.Null);
    }

    [Test]
    public void ToTelegramHtml_WhenMessageHasEntities_ReturnsHtmlMarkup()
    {
        var message = new Message
        {
            Text = "abc",
            Entities = [new MessageEntity { Type = MessageEntityType.Bold, Offset = 0, Length = 3 }],
        };

        Assert.That(message.ToTelegramHtml(), Is.EqualTo("<b>abc</b>"));
    }

    [Test]
    public void ToTelegramHtml_WhenMessageHasNoTextAndCaption_ReturnsNull()
    {
        var message = new Message();

        Assert.That(message.ToTelegramHtml(), Is.Null);
    }

    [Test]
    public void ToTelegramHtml_WhenMessageIsNull_ReturnsNull()
    {
        Message? message = null;

        Assert.That(message.ToTelegramHtml(), Is.Null);
    }

    [Test]
    public void ToPlainText_WhenHtmlHasMarkup_ReturnsPlainText()
    {
        Assert.That("<b>a &amp; b</b>".ToPlainText(), Is.EqualTo("a & b"));
    }

    [Test]
    public void ToPlainLength_WhenHtmlHasMarkup_CountsOnlyText()
    {
        Assert.That("<b>abc</b>".ToPlainLength(), Is.EqualTo(3));
    }

    [Test]
    public void ToPlainLength_WhenHtmlIsNullOrEmpty_ReturnsZero()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(((string?)null).ToPlainLength(), Is.Zero);
            Assert.That(string.Empty.ToPlainLength(), Is.Zero);
        }
    }
}
