namespace TBotPlatform.Tests.Contracts.Bots.Buttons;

using NUnit.Framework;
using TBotPlatform.Contracts.Bots.Buttons;
using TBotPlatform.Contracts.Bots.Constant;

[TestFixture]
public class MainButtonTests
{
    [Test]
    public void Constructor_WhenButtonIsNull_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => new MainButton(null!));

    [Test]
    public void Constructor_WhenButtonIsEmpty_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => new MainButton(""));

    [Test]
    public void Constructor_WhenButtonIsWhitespace_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => new MainButton("   "));

    [Test]
    public void Constructor_WhenButtonLengthIsWithinLimit_SetsButtonName()
    {
        var button = new MainButton("TestButton");

        Assert.That(button.ButtonName, Is.EqualTo("TestButton"));
    }

    [Test]
    public void Constructor_WhenButtonLengthExceedsLimit_TruncatesButtonName()
    {
        var longButton = new string('A', ButtonsRuleConstant.ButtonsRuleNameLength + 10);
        var button = new MainButton(longButton);

        Assert.That(button.ButtonName, Is.EqualTo(longButton));
        Assert.That(button.ButtonName, Has.Length.EqualTo(longButton.Length));
    }

    [Test]
    public void Constructor_WhenButtonLengthEqualsLimit_SetsButtonName()
    {
        var exactLengthButton = new string('A', ButtonsRuleConstant.ButtonsRuleNameLength);
        var button = new MainButton(exactLengthButton);

        Assert.That(button.ButtonName, Is.EqualTo(exactLengthButton));
    }

    [Test]
    public void ButtonName_IsReadOnly()
    {
        var button = new MainButton("Test");
        var property = typeof(MainButton).GetProperty(nameof(MainButton.ButtonName));

        Assert.That(property?.SetMethod?.IsPrivate, Is.True);
    }
}
