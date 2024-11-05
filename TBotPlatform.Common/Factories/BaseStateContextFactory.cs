#nullable enable
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Common.Factories;

internal abstract class BaseStateContextFactory
{
    internal const string ErrorText = "🆘 Произошла ошибка при обработке запроса";

    internal abstract Task RequestAsync<T>(IStateContext stateContext, T user, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase;
}