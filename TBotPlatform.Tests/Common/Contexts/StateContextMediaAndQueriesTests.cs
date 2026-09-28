using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Moq;
using NUnit.Framework;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.Exceptions;
using TBotPlatform.Contracts.Bots.FileDatas;
using TBotPlatform.Contracts.Bots.Markups;
using TBotPlatform.Contracts.Bots.Markups.InlineMarkups;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace TBotPlatform.Tests.Common.Contexts;

/// <summary>
/// Проверяет валидацию и проброс параметров в новых методах StateContext: альбомы, inline-разметка, платежи и inline-режим.
/// </summary>
[TestFixture]
public class StateContextMediaAndQueriesTests
{
    private const long ChatIdValue = 777;

    [Test]
    public async Task SendMediaGroup_WhenFilesCountIsOutOfRange_ThrowsBeforeAnyRequest()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        Assert.ThrowsAsync<MediaGroupCountException>(
            () => stateContext.SendMediaGroup([CreateFileData()], CancellationToken.None)
            );

        Assert.ThrowsAsync<MediaGroupCountException>(
            () => stateContext.SendMediaGroup([.. Enumerable.Range(0, 11).Select(_ => CreateFileData())], CancellationToken.None)
            );

        Assert.ThrowsAsync<MediaGroupCountException>(
            () => stateContext.SendMediaGroup(null!, CancellationToken.None)
            );

        telegramContext.Verify(
            x => x.SendRequest(It.IsAny<SendMediaGroupRequest>(), It.IsAny<CancellationToken>()),
            Times.Never
            );
    }

    [Test]
    public async Task SendMediaGroup_WhenFileHasNoBytes_Throws()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        var mediaDatas = new List<FileDataBase>
        {
            CreateFileData(),
            new FileData { Name = "broken.png", Bytes = null!, Size = 0, FileId = "broken" },
        };

        Assert.ThrowsAsync<MediaGroupDataException>(() => stateContext.SendMediaGroup(mediaDatas, CancellationToken.None));
    }

    [Test]
    public async Task SendMediaGroup_WhenTwoFilesPassed_SendsAlbumWithNotificationFlag()
    {
        SendMediaGroupRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendMediaGroupRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message[]>, CancellationToken>((request, _) => captured = (SendMediaGroupRequest)request)
            .ReturnsAsync(Array.Empty<Message>());

        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        var messages = await stateContext.SendMediaGroup([CreateFileData(), CreateFileData()], disableNotification: true, CancellationToken.None);

        Assert.That(messages, Is.Empty);
        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.ChatId.Identifier, Is.EqualTo(ChatIdValue));
        Assert.That(captured.DisableNotification, Is.True);
        Assert.That(captured.Media.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateInlineMarkup_WhenCallbackQueryIsMissing_Throws()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        Assert.Throws<CallbackQueryArgException>(() => stateContext.UpdateInlineMarkup(CreateMassiveList(), CancellationToken.None));
    }

    [Test]
    public async Task UpdateInlineMarkup_WhenCallbackQueryIsPresent_EditsMarkupOfThatMessage()
    {
        EditMessageReplyMarkupRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<EditMessageReplyMarkupRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) => captured = (EditMessageReplyMarkupRequest)request)
            .ReturnsAsync(new Message());

        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        stateContext.CreateStateContext(CreateCallbackUpdate(messageId: 55), markupNextState: null);

        await stateContext.UpdateInlineMarkup(CreateMassiveList(), CancellationToken.None);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.MessageId, Is.EqualTo(55));
        Assert.That(captured.ChatId.Identifier, Is.EqualTo(ChatIdValue));
        Assert.That(captured.ReplyMarkup, Is.Not.Null);
    }

    [Test]
    public async Task SendInvoice_WhenChatIdIsMissing_Throws()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, chatId: 0);

        Assert.Throws<ChatIdArgException>(
            () => stateContext.SendInvoice(
                "title",
                "description",
                "payload",
                "USD",
                [new LabeledPrice { Label = "item", Amount = 100 }],
                providerToken: null,
                providerData: null,
                CancellationToken.None
                )
            );
    }

    [Test]
    public async Task AnswerInlineQuery_WhenInlineQueryIdIsBlank_Throws()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        Assert.Throws<ArgumentException>(
            () => stateContext.AnswerInlineQuery(" ", [], cacheTime: null, nextOffset: null, CancellationToken.None)
            );
    }

    [Test]
    public async Task AnswerPreCheckoutQuery_WhenIdIsBlank_Throws()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        Assert.Throws<ArgumentException>(
            () => stateContext.AnswerPreCheckoutQuery(string.Empty, errorMessage: null, CancellationToken.None)
            );
    }

    [Test]
    public async Task SendMediaGroup_WhenMediaGroupTypeIsVideo_SendsVideoAlbum()
    {
        SendMediaGroupRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendMediaGroupRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message[]>, CancellationToken>((request, _) => captured = (SendMediaGroupRequest)request)
            .ReturnsAsync(Array.Empty<Message>());

        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        await stateContext.SendMediaGroup(
            [CreateFileData(), CreateFileData()],
            MediaGroupType.Video,
            CancellationToken.None
            );

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Media, Has.All.InstanceOf<InputMediaVideo>());
    }

    [Test]
    public async Task SendMediaGroup_WhenMediaGroupTypeIsDocument_SendsDocumentAlbum()
    {
        SendMediaGroupRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendMediaGroupRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message[]>, CancellationToken>((request, _) => captured = (SendMediaGroupRequest)request)
            .ReturnsAsync(Array.Empty<Message>());

        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        await stateContext.SendMediaGroup(
            [CreateFileData(), CreateFileData()],
            MediaGroupType.Document,
            disableNotification: true,
            CancellationToken.None
            );

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Media, Has.All.InstanceOf<InputMediaDocument>());
        Assert.That(captured.DisableNotification, Is.True);
    }

    [Test]
    public async Task SendTextMessageWithEntities_WhenEntitiesAreEmpty_DoesNotSendRequest()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        var message = await stateContext.SendTextMessageWithEntities("текст", [], CancellationToken.None);

        Assert.That(message, Is.Null);

        telegramContext.Verify(
            x => x.SendRequest(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()),
            Times.Never
            );
    }

    [Test]
    public async Task SendTextMessageWithEntities_WhenEntitiesPassed_SendsThemWithoutParseMode()
    {
        SendMessageRequest? captured = null;

        var telegramContext = CreateTelegramContextMock();

        telegramContext
            .Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) => captured = (SendMessageRequest)request)
            .ReturnsAsync(new Message());

        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        var entities = new List<MessageEntity>
        {
            new() { Type = MessageEntityType.Bold, Offset = 0, Length = 5 },
        };

        await stateContext.SendTextMessageWithEntities("текст", entities, disableNotification: true, CancellationToken.None);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Entities, Is.EquivalentTo(entities));
        Assert.That(captured.ParseMode, Is.EqualTo(ParseMode.None));
        Assert.That(captured.DisableNotification, Is.True);
    }

    [Test]
    public async Task GetMessageLink_WhenChannelPostHasUsername_ReturnsPublicLink()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        stateContext.CreateStateContext(
            new Update
            {
                Id = 1,
                ChannelPost = new Message
                {
                    Id = 42,
                    Chat = new Chat { Id = -1001234567890, Type = ChatType.Channel, Username = "test_channel" },
                },
            },
            markupNextState: null
            );

        Assert.That(stateContext.GetMessageLink(), Is.EqualTo("https://t.me/test_channel/42"));
    }

    [Test]
    public async Task GetMessageLink_WhenUpdateIsMissing_ReturnsNull()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        Assert.That(stateContext.GetMessageLink(), Is.Null);
    }

    [Test]
    public async Task GetMessageLink_WhenPrivateChatMessage_ReturnsNull()
    {
        var telegramContext = CreateTelegramContextMock();
        await using var stateContext = CreateStateContext(telegramContext.Object, ChatIdValue);

        stateContext.CreateStateContext(
            new Update
            {
                Id = 1,
                Message = new Message
                {
                    Id = 42,
                    Chat = new Chat { Id = ChatIdValue, Type = ChatType.Private },
                },
            },
            markupNextState: null
            );

        Assert.That(stateContext.GetMessageLink(), Is.Null);
    }

    private static FileData CreateFileData() => new()
    {
        Name = "photo.png",
        Bytes = [1, 2, 3],
        Size = 3,
        FileId = "file-id",
    };

    private static InlineMarkupMassiveList CreateMassiveList() =>
    [
        new InlineMarkupMassive
        {
            InlineMarkups = [new InlineMarkupState("Кнопка", "MainMenu")],
            ButtonsPerRow = 1,
        },
    ];

    private static Update CreateCallbackUpdate(int messageId) => new()
    {
        Id = 1,
        CallbackQuery = new CallbackQuery
        {
            Id = "callback-id",
            From = new User { Id = 42, IsBot = false, FirstName = "user" },
            Message = new Message
            {
                Id = messageId,
                Chat = new Chat { Id = ChatIdValue, Type = ChatType.Private },
            },
        },
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
