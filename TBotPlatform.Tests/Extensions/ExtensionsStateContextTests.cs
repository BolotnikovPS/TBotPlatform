#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Moq;
using TBotPlatform.Common;
using TBotPlatform.Common.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Abstractions.Queues;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Contracts.Bots.States;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Tests.Extensions;

/// <summary>
/// Проверяет расширение <see cref="Extensions.TryGetStateResult"/>: результат состояния
/// доступен только для реального <see cref="StateContext"/>, для любого другого контекста — false.
/// </summary>
[TestFixture]
public class ExtensionsStateContextTests
{
    private const long ChatIdValue = 100;

    [Test]
    public async Task TryGetStateResult_WhenContextIsStateContext_ReturnsAssignedStateResult()
    {
        await using var stateContext = CreateStateContext();
        var stateResult = new StateResult { IsNeedUpdateMarkup = true, NextStateName = "NextState", Data = "payload" };
        stateContext.StateResult = stateResult;

        var found = stateContext.TryGetStateResult(out var result);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(found, Is.True);
            Assert.That(result, Is.SameAs(stateResult));
            Assert.That(result!.NextStateName, Is.EqualTo("NextState"));
        }
    }

    [Test]
    public async Task TryGetStateResult_WhenStateResultNotChanged_ReturnsDefaultStateResult()
    {
        await using var stateContext = CreateStateContext();

        var found = stateContext.TryGetStateResult(out var result);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(found, Is.True);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsNeedUpdateMarkup, Is.False);
        }
    }

    [Test]
    public void TryGetStateResult_WhenContextIsNotStateContext_ReturnsFalseAndNull()
    {
        var stateContext = Mock.Of<IStateContextMinimal>();

        var found = stateContext.TryGetStateResult(out var result);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(found, Is.False);
            Assert.That(result, Is.Null);
        }
    }

    private static StateContext CreateStateContext()
    {
        var scope = new ServiceCollection().BuildServiceProvider().CreateAsyncScope();

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

        return new StateContext(
            scope,
            stateHistory: null,
            Mock.Of<IStateBindFactory>(),
            telegramContext.Object,
            Mock.Of<IDelayQueue>(),
            new RecyclableMemoryStreamManager(),
            ChatIdValue
            );
    }
}
