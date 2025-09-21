#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Contexts;
using TBotPlatform.Contracts.Abstractions.Builder;
using TBotPlatform.Contracts.Abstractions.Contexts;
using TBotPlatform.Contracts.Abstractions.Handlers;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots.Config;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Builder;

internal partial class BotBuilder(IServiceCollection serviceCollection, IBotPlatformBuilder botPlatformBuilder, TelegramSettings telegramSettings) : IBotBuilder
{
    private Action<HttpClient>? HttpClient { get; set; }
    private Type? Log { get; set; }
    private List<Type>? PotentialStateTypes { get; set; }
    private Type? ReceivingHandlerType { get; set; }

    public IBotBuilder AddTelegramContext<TLog>(Action<HttpClient>? httpClient = null)
        where TLog : ITelegramContextLog
    {
        if (HttpClient.IsNotNull() || Log.IsNotNull())
        {
            throw new InvalidOperationException("Контекст telegram ранее был добавлен.");
        }

        HttpClient = httpClient;
        Log = typeof(TLog);

        return this;
    }

    public IBotBuilder AddTelegramContext(Action<HttpClient>? httpClient = null)
    {
        if (HttpClient.IsNotNull() || Log.IsNotNull())
        {
            throw new InvalidOperationException("Контекст telegram ранее был добавлен.");
        }

        HttpClient = httpClient;
        Log = typeof(TelegramContextLog);

        return this;
    }

    public IBotBuilder AddStates(Assembly executingAssembly)
    {
        if (executingAssembly.IsNull())
        {
            throw new ArgumentNullException(nameof(executingAssembly));
        }

        var potentialStateTypes = executingAssembly
                                 ?.GetTypes()
                                 ?.Where(z => z.IsDefined(typeof(StateActivatorBaseAttribute), inherit: false))
                                 ?.OrderBy(z => z.FullName)
                                 ?.ToList();

        if (potentialStateTypes.IsNull())
        {
            throw new InvalidDataException("Отсутствуют потенциальные состояния.");
        }

        AddStates(potentialStateTypes!);

        return this;
    }

    public IBotBuilder AddStates(List<Type> potentialStateTypes)
    {
        if (potentialStateTypes.IsNull())
        {
            throw new ArgumentNullException(nameof(potentialStateTypes));
        }

        if (PotentialStateTypes.IsNull())
        {
            PotentialStateTypes = potentialStateTypes;

            return this;
        }

        PotentialStateTypes!.AddRange(potentialStateTypes);

        return this;
    }

    public IBotBuilder AddReceivingHandler<T>()
        where T : IStartReceivingHandler
    {
        if (ReceivingHandlerType.IsNotNull())
        {
            throw new InvalidOperationException("Обработчик событий от telegram ранее был добавлен.");
        }

        ReceivingHandlerType = typeof(T);
        return this;
    }

    public IBotPlatformBuilder Build()
    {
        if (telegramSettings.IsNull() || Log.IsNull())
        {
            throw new InvalidOperationException("Отсутствует контекст telegram.");
        }

        if (ReceivingHandlerType.IsNull())
        {
            throw new InvalidOperationException("Отсутствует обработчик событий от telegram.");
        }

        AddBotTelegramContext();
        AddBotStates();

        serviceCollection.AddKeyedScoped(typeof(IStartReceivingHandler), telegramSettings.BotName, ReceivingHandlerType!);

        return botPlatformBuilder;
    }
}
