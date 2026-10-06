using NUnit.Framework;
using TBotPlatform.Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Extensions;

[TestFixture]
public class ExtensionsMessageUserDataTests
{
    private static readonly User AnyUser = new() { Id = 42, IsBot = false, FirstName = "user" };
    private static readonly Chat AnyChat = new() { Id = 777, Type = ChatType.Private };

    [Test]
    public void TryGetMessageUserData_WhenUpdateTypeUnsupported_ReturnsFalse()
    {
        var update = new Update { Id = 1 };

        var result = update.TryGetMessageUserData(out var telegramMessageUserData);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(telegramMessageUserData, Is.Null);
        }
    }

    [Test]
    public void TryGetMessageUserData_WhenGuestMessage_ReturnsUserAndChat()
    {
        var update = new Update
        {
            Id = 1,
            GuestMessage = new Message { From = AnyUser, Chat = AnyChat },
        };

        var result = update.TryGetMessageUserData(out var telegramMessageUserData);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(telegramMessageUserData!.UserOrNull, Is.EqualTo(AnyUser));
            Assert.That(telegramMessageUserData.ChatOrNull, Is.EqualTo(AnyChat));
        }
    }

    [Test]
    public void TryGetMessageUserData_WhenManagedBot_ReturnsUserWithoutChat()
    {
        var update = new Update
        {
            Id = 1,
            ManagedBot = new ManagedBotUpdated { User = AnyUser, Bot = new User { Id = 2, IsBot = true, FirstName = "bot" } },
        };

        var result = update.TryGetMessageUserData(out var telegramMessageUserData);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(telegramMessageUserData!.UserOrNull, Is.EqualTo(AnyUser));
            Assert.That(telegramMessageUserData.ChatOrNull, Is.Null);
        }
    }

    [Test]
    public void TryGetMessageUserData_WhenSubscription_ReturnsUserWithoutChat()
    {
        var update = new Update
        {
            Id = 1,
            Subscription = new BotSubscriptionUpdated { User = AnyUser, InvoicePayload = "payload" },
        };

        var result = update.TryGetMessageUserData(out var telegramMessageUserData);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(telegramMessageUserData!.UserOrNull, Is.EqualTo(AnyUser));
            Assert.That(telegramMessageUserData.ChatOrNull, Is.Null);
        }
    }

    [Test]
    public void TryGetMessageUserData_WhenStoppedMessageGeneration_ReturnsChatWithoutUser()
    {
        var update = new Update
        {
            Id = 1,
            StoppedMessageGeneration = new MessageGenerationStopped { Chat = AnyChat, DraftId = 5 },
        };

        var result = update.TryGetMessageUserData(out var telegramMessageUserData);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(telegramMessageUserData!.UserOrNull, Is.Null);
            Assert.That(telegramMessageUserData.ChatOrNull, Is.EqualTo(AnyChat));
        }
    }

    [Test]
    public void TryGetMessageUserData_WhenPoll_ReturnsFalse()
    {
        var update = new Update
        {
            Id = 1,
            Poll = new Poll { Id = "poll", Question = "q", Options = [] },
        };

        var result = update.TryGetMessageUserData(out var telegramMessageUserData);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(telegramMessageUserData, Is.Null);
        }
    }
}
