using Microsoft.Extensions.DependencyInjection;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.Contexts.AsyncDisposable;
using TBotPlatform.Contracts.Abstractions.Factories;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal class MenuButtonFactory(IServiceScopeFactory serviceScopeFactory) : IMenuButtonFactory
{
    public Task UpdateMainButtonsByStateAsync<T>(T user, IStateContextMinimal stateContext, StateHistory stateHistory, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(stateHistory);
        ArgumentNullException.ThrowIfNull(stateHistory.MenuStateType);

        return UpdateMainButtonsByStateAsync(user, stateContext, stateHistory.MenuStateType, cancellationToken);
    }

    public async Task UpdateMainButtonsByStateAsync<T>(T user, IStateContextMinimal stateContext, Type menuStateType, CancellationToken cancellationToken)
        where T : UserBase
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stateContext);
        ArgumentNullException.ThrowIfNull(menuStateType);

        var isMenuType = menuStateType.GetInterfaces().Any(x => x.Name == nameof(IMenuButton));

        if (!isMenuType)
        {
            throw new($"Класс {menuStateType.Name} не наследуется от {nameof(IMenuButton)}");
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var menuButtons = scope.ServiceProvider.GetRequiredService(menuStateType) as IMenuButton;

        if (menuButtons.IsNull())
        {
            throw new("Не смог активировать меню");
        }

        var mainButtons = await menuButtons!.GetMainButtonsAsync(user);
        await stateContext.UpdateMainButtonsAsync(mainButtons, cancellationToken);
    }
}