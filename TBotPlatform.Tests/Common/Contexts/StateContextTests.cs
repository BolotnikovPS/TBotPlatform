using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Moq;
using NUnit.Framework;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.FileDatas;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Common.Contexts;

/// <summary>
/// Проверяет, что StateContext передает в Telegram.Bot именно те параметры, которые получил от вызывающего кода.
/// </summary>
[TestFixture]
public class StateContextTests
{
    private const long ChatIdValue = 777;

    [Test]
    public async Task LeaveChat_WhenChatIdToLeavePassed_LeavesExactlyThatChat()
    {
        LeaveChatRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<LeaveChatRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((request, _) => captured = (LeaveChatRequest)request)
            .ReturnsAsync(true);

        await using var stateContext = CreateStateContext(telegramContext.Object, chatId: ChatIdValue);

        await stateContext.LeaveChat(chatIdToLeave: 999, CancellationToken.None);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.ChatId.Identifier, Is.EqualTo(999));
    }

    [Test]
    public async Task SendPhoto_WhenDisableNotificationRequested_PassesFlagToRequest()
    {
        SendPhotoRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendPhotoRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) => captured = (SendPhotoRequest)request)
            .ReturnsAsync(new Message());

        await using var stateContext = CreateStateContext(telegramContext.Object, chatId: ChatIdValue);

        await stateContext.SendPhoto(CreateFileData(), disableNotification: true, CancellationToken.None);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.DisableNotification, Is.True);
        Assert.That(captured.ChatId.Identifier, Is.EqualTo(ChatIdValue));
    }

    [Test]
    public async Task SendTextMessage_WhenTextIsEmpty_ReturnsNullMessageInsteadOfNullTask()
    {
        var telegramContext = CreateTelegramContextMock();

        await using var stateContext = CreateStateContext(telegramContext.Object, chatId: ChatIdValue);

        var message = await stateContext.SendTextMessage(string.Empty, CancellationToken.None);

        Assert.That(message, Is.Null);
        telegramContext.Verify(
            x => x.SendRequest(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()),
            Times.Never
            );
    }

    [Test]
    public async Task SendLongTextMessage_WhenTextIsLong_KeepsParseModeInEveryChunk()
    {
        var captured = new List<SendMessageRequest>();

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) => captured.Add((SendMessageRequest)request))
            .ReturnsAsync(new Message());

        await using var stateContext = CreateStateContext(telegramContext.Object, chatId: ChatIdValue);

        await stateContext.SendLongTextMessage(new string('a', 5000), disableNotification: false, CancellationToken.None);

        Assert.That(captured, Has.Count.EqualTo(2));
        Assert.That(captured.Select(x => x.ParseMode), Is.All.EqualTo(ParseMode.Html));
    }

    private static FileData CreateFileData() => new()
    {
        Name = "photo.png",
        Bytes = [1, 2, 3],
        Size = 3,
        FileId = "file-id",
    };

    private static Mock<ITelegramContext> CreateTelegramContextMock()
    {
        var telegramContext = new Mock<ITelegramContext>();

        telegramContext
            .Setup(x => x.GetBotSetting())
            .Returns(new TBotSetting
            {
                BotName = "test",
                Token = "1:token",
                ParseMode = ParseMode.Html,
            });

        telegramContext
            .Setup(x => x.CurrentOperation)
            .Returns(Guid.NewGuid());

        return telegramContext;
    }

    private static StateContext CreateStateContext(ITelegramContext telegramContext, long chatId)
    {
        var scope = new ServiceCollection().BuildServiceProvider().CreateAsyncScope();

        return new StateContext(
            scope,
            stateHistory: null,
            Mock.Of<IStateBindFactory>(),
            telegramContext,
            Mock.Of<IDelayQueue>(),
            new RecyclableMemoryStreamManager(),
            chatId
            );
    }
}
