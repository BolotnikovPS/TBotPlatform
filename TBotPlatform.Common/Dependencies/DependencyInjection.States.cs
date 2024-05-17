using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Attibutes;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddStates(
        this IServiceCollection services,
        Assembly executingAssembly,
        string botType
        )
    {
        var result = executingAssembly
                    .GetTypes()
                    .Where(z => z.IsDefined(typeof(StateActivatorBaseAttribute), false))
                    .OrderBy(z => z.FullName)
                    .ToList();

        if (!result.CheckAny())
        {
            throw new Exception("Нет состояний");
        }

        var stateInterfaceName = typeof(IState<UserBase>).Name;
        var menuButtonInterfaceName = typeof(IMenuButton<UserBase>).Name;

        var states = new List<StateFactoryData>();

        foreach (var type in result)
        {
            var isIStateType = type.GetInterfaces().Any(x => x.Name == stateInterfaceName);

            if (!isIStateType)
            {
                throw new Exception($"Класс {type.Name} содержит атрибут {nameof(StateActivatorBaseAttribute)} но не наследуется от {stateInterfaceName}");
            }

            var attr =
                type.GetCustomAttributes(typeof(StateActivatorBaseAttribute), true).FirstOrDefault() as
                    StateActivatorBaseAttribute;

            if (!attr!.OnlyForBot.In("None", botType)
                && attr.OnlyForBot.CheckAny()
               )
            {
                continue;
            }

            if (attr.ButtonsTypes.CheckAny())
            {
                var checkButtons = states
                                  .Where(c => c.ButtonsTypes.CheckAny())
                                  .Where(x => x.ButtonsTypes.Any(q => attr.ButtonsTypes.Any(z => z.ToString() == q)))
                                  .ToList();

                if (checkButtons.CheckAny())
                {
                    throw new Exception($"В состоянии {type.Name} имеются дубли по ButtonsTypes с состояниями {string.Join(",", checkButtons)}");
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
                    throw new Exception($"В состоянии {type.Name} имеются дубли по TextsTypes с состояниями {string.Join(",", checkTexts)}");
                }
            }

            if (attr.IsLockUserState)
            {
                var lockStates = states
                                .Where(x => x.IsLockUserState)
                                .ToList();

                switch (lockStates.Count)
                {
                    case < 1:
                        throw new Exception($"Нет состояния определяющее параметр {nameof(StateActivatorBaseAttribute.IsLockUserState)} атрибута {nameof(StateActivatorBaseAttribute)}");

                    case > 1:
                        throw new Exception($"В состоянии {type.Name} имеются дубли по LockUserState с состояниями {string.Join(",", lockStates)}");
                }
            }

            if (attr.MenuType.CheckAny())
            {
                var isIMenuButtonType = attr.MenuType.GetInterfaces().Any(x => x.Name == menuButtonInterfaceName);

                if (!isIMenuButtonType)
                {
                    throw new Exception($"Класс {attr.MenuType.Name} находится в атрибуте {nameof(StateActivatorBaseAttribute)} но не наследуется от {menuButtonInterfaceName}");
                }
            }

            states.Add(
                new StateFactoryData(
                    type.Name,
                    attr.MenuType?.Name,
                    attr.IsInlineState,
                    attr.IsLockUserState,
                    attr.ButtonsTypes,
                    attr.TextsTypes,
                    attr.CommandsTypes
                    )
                );

            services.AddScoped(type);
        }

        var stateStartCount = states
           .Count(
                z => z.CommandsTypes.CheckAny()
                     && z.CommandsTypes.Any(s => s.In("/start"))
                );

        switch (stateStartCount)
        {
            case < 1:
                throw new Exception("Нет состояний определяющих комманду /start");

            case > 1:
                throw new Exception("Состояний определяющих комманду /start больше одного");
        }

        var statesCollection = new StateFactoryDataCollection(states);
        services.AddSingleton(statesCollection);

        return services;
    }
}