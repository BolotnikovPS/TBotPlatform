using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Tests.Contracts.Bots.Users;

[TestFixture]
public class UserBaseTests
{
    [Test]
    public void ToString_WhenUserNameIsSet_ReturnsUserNameWithTgUserId()
    {
        var user = new TestUser
        {
            UserName = "testuser",
            TgUserId = 12345L
        };

        var result = user.ToString();

        Assert.That(result, Is.EqualTo("@testuser (12345)"));
    }

    [Test]
    public void ToString_WhenUserNameIsNullAndFirstNameLastNameAreSet_ReturnsFirstNameLastName()
    {
        var user = new TestUser
        {
            UserName = null,
            FirstName = "John",
            LastName = "Doe",
            TgUserId = 12345L
        };

        var result = user.ToString();

        Assert.That(result, Is.EqualTo("John Doe (12345)"));
    }

    [Test]
    public void ToString_WhenUserNameIsNullAndLastNameIsNull_ReturnsFirstNameOnly()
    {
        var user = new TestUser
        {
            UserName = null,
            FirstName = "John",
            LastName = null,
            TgUserId = 12345L
        };

        var result = user.ToString();

        Assert.That(result, Is.EqualTo("John (12345)"));
    }

    [Test]
    public void ToString_WhenUserNameIsNullAndFirstNameIsNull_ReturnsLastNameOnly()
    {
        var user = new TestUser
        {
            UserName = null,
            FirstName = null,
            LastName = "Doe",
            TgUserId = 12345L
        };

        var result = user.ToString();

        Assert.That(result, Is.EqualTo(" Doe (12345)"));
    }

    [Test]
    public void ToString_WhenUserNameIsNullAndBothNamesAreNull_ReturnsOnlyTgUserId()
    {
        var user = new TestUser
        {
            UserName = null,
            FirstName = null,
            LastName = null,
            TgUserId = 12345L
        };

        var result = user.ToString();

        Assert.That(result, Is.EqualTo(" (12345)"));
    }

    [Test]
    public void ToString_WhenUserNameIsEmpty_ReturnsFirstNameLastName()
    {
        var user = new TestUser
        {
            UserName = "",
            FirstName = "John",
            LastName = "Doe",
            TgUserId = 12345L
        };

        var result = user.ToString();

        Assert.That(result, Is.EqualTo("John Doe (12345)"));
    }

    private class TestUser : UserBase
    {
        public override bool IsAdmin() => false;
    }
}
