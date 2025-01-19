using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Abstractions.State;
using TBotPlatform.Contracts.Attributes;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Contracts.Bots.Users;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddStates(this IServiceCollection services, string botType = "", params Type[] potentialStateTypes)
        => services.AddStates(botTypes: [botType,], potentialStateTypes);

    public static IServiceCollection AddStates(this IServiceCollection services, Assembly executingAssembly, string botType = "")
        => services.AddStates(executingAssembly, botTypes: botType);

    public static IServiceCollection AddStates(this IServiceCollection services, Assembly executingAssembly, params string[] botTypes)
    {
        if (executingAssembly.IsNull())
        {
            throw new ArgumentNullException(nameof(executingAssembly));
        }

        var potentialStateTypes = executingAssembly
                                 .GetTypes()
                                 .Where(z => z.IsDefined(typeof(StateActivatorBaseAttribute), inherit: false))
                                 .OrderBy(z => z.FullName)
                                 .ToArray();

        return services.AddStates(botTypes, potentialStateTypes);
    }

    public static IServiceCollection AddStates(this IServiceCollection services, string[] botTypes, params Type[] potentialStateTypes)
    {
        if (potentialStateTypes.IsNull())
        {
            throw new("Нет состояний");
        }

        var stateInterfaceName = typeof(IState<UserBase>).Name;
        const string menuButtonInterfaceName = nameof(IMenuButton);

        var states = new List<StateFactoryData>();

        foreach (var stateType in potentialStateTypes)
        {
            var isIStateType = stateType.GetInterfaces().Any(x => x.Name == stateInterfaceName);

            if (!isIStateType)
            {
                throw new($"Класс {stateType.Name} содержит атрибут {nameof(StateActivatorBaseAttribute)} но не наследуется от {stateInterfaceName}");
            }

            var attr = stateType.GetCustomAttributes(typeof(StateActivatorBaseAttribute), inherit: true).FirstOrDefault() as StateActivatorBaseAttribute;

            if (attr!.OnlyForBot.CheckAny() && attr.OnlyForBot.NotIn("None", ""))
            {
                if (attr.OnlyForBot.NotIn(botTypes))
                {
                    continue;
                }
            }

            if (attr.ButtonsTypes.CheckAny())
            {
                var checkButtons = states
                                  .Where(c => c.ButtonsTypes.CheckAny())
                                  .Where(x => x.ButtonsTypes.Any(q => attr.ButtonsTypes.Any(z => z.ToString() == q)))
                                  .ToList();

                if (checkButtons.CheckAny())
                {
                    throw new($"В состоянии {stateType.Name} имеются дубли по ButtonsTypes с состояниями {string.Join(",", checkButtons)}");
                }
            }

            if (attr.TextsTypes.CheckAny())
            {
                var checkTexts = states
                                .Where(c => c.TextsTypes.CheckAny())
                                .Where(x => x.TextsTypes.Any(q => attr.TextsTypes.Any(z => z.ToString() == q)))
                                .ToList();

                if (checkTexts.CheckAny())
                {
                    throw new($"В состоянии {stateType.Name} имеются дубли по TextsTypes с состояниями {string.Join(",", checkTexts)}");
                }
            }

            if (attr.MenuType.IsNotNull())
            {
                var isIMenuButtonType = attr.MenuType.GetInterfaces().Any(x => x.Name == menuButtonInterfaceName);

                if (!isIMenuButtonType)
                {
                    throw new($"Класс {attr.MenuType.Name} не наследуется от {menuButtonInterfaceName}");
                }
            }

            if (attr.MenuType.IsNotNull() && attr.IsInlineState)
            {
                throw new(
                    $"Класс {stateType.Name} не должен определять атрибут {nameof(StateActivatorBaseAttribute)} в котором единовременно определены {nameof(attr.IsInlineState)} = true и {nameof(attr.MenuType)} != null");
            }

            var newState = new StateFactoryData(
                stateType.Name,
                attr.MenuType?.Name,
                attr.IsInlineState,
                attr.IsLockUserState,
                attr.IsRegistrationState,
                attr.ButtonsTypes,
                attr.TextsTypes,
                attr.CommandsTypes
                );

            states.Add(newState);

            services.AddScoped(stateType);

            if (attr.MenuType.IsNotNull())
            {
                services.AddScoped(attr!.MenuType!);
            }
        }

        var lockStates = states
                        .Where(x => x.IsLockUserState)
                        .ToList();

        if (lockStates.Count > 1)
        {
            throw new(
                $"Имеются дубли по параметру {nameof(StateActivatorBaseAttribute.IsLockUserState)} атрибута {nameof(StateActivatorBaseAttribute)}. Список состояний: {string.Join(",", lockStates)}");
        }

        var registrationStates = states
                                .Where(x => x.IsRegistrationState)
                                .ToList();

        if (registrationStates.Count > 1)
        {
            throw new(
                $"Имеются дубли по параметру {nameof(StateActivatorBaseAttribute.IsRegistrationState)} атрибута {nameof(StateActivatorBaseAttribute)}. Список состояний: {string.Join(",", registrationStates)}");
        }

        var stateStartCount = states
           .Count(
                z => z.CommandsTypes.CheckAny()
                     && z.CommandsTypes.Any(s => s.In(CommandTypesConstant.StartCommand))
                );

        switch (stateStartCount)
        {
            case < 1:
                throw new("Нет состояний определяющих команду /start");

            case > 1:
                throw new("Состояний определяющих команду /start больше одного");
        }

        var statesCollection = new StateFactoryDataCollection(states);
        services.ConfigureOptions(statesCollection);

        return services;
    }
}