using NUnit.Framework;
using TBotPlatform.Extension;
using Telegram.Bot.Types;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsChatMembersTests
{
    private static readonly User AnyUser = new() { Id = 42, IsBot = false, FirstName = "user" };

    [Test]
    public void IsAdminOrCreator_WhenOwnerOrAdministrator_ReturnsTrue()
    {
        ChatMember owner = new ChatMemberOwner { User = AnyUser };
        ChatMember administrator = new ChatMemberAdministrator { User = AnyUser };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(owner.IsAdminOrCreator(), Is.True);
            Assert.That(administrator.IsAdminOrCreator(), Is.True);
        }
    }

    [Test]
    public void IsAdminOrCreator_WhenMemberOrNull_ReturnsFalse()
    {
        ChatMember member = new ChatMemberMember { User = AnyUser };
        ChatMember? absent = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(member.IsAdminOrCreator(), Is.False);
            Assert.That(absent.IsAdminOrCreator(), Is.False);
        }
    }

    [Test]
    public void IsInChat_WhenMemberOrRestrictedAndStillMember_ReturnsTrue()
    {
        ChatMember member = new ChatMemberMember { User = AnyUser };
        ChatMember restricted = new ChatMemberRestricted { User = AnyUser, IsMember = true };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(member.IsInChat(), Is.True);
            Assert.That(restricted.IsInChat(), Is.True);
        }
    }

    [Test]
    public void IsInChat_WhenRestrictedButNotMember_ReturnsFalse()
    {
        ChatMember restricted = new ChatMemberRestricted { User = AnyUser, IsMember = false };

        Assert.That(restricted.IsInChat(), Is.False);
    }

    [Test]
    public void IsInChat_WhenLeftOrBannedOrNull_ReturnsFalse()
    {
        ChatMember left = new ChatMemberLeft { User = AnyUser };
        ChatMember banned = new ChatMemberBanned { User = AnyUser };
        ChatMember? absent = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(left.IsInChat(), Is.False);
            Assert.That(banned.IsInChat(), Is.False);
            Assert.That(absent.IsInChat(), Is.False);
        }
    }
}
