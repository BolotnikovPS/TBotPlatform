using TBotPlatform.Contracts.Abstractions;
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory<T>
{
    private StateHistory<T> CreateContextFunc()
    {
        var state = stateFactoryDataCollection
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(x => x.In("/start"))
                );

        return Convert(state!);
    }

    private StateHistory<T> Convert(StateFactoryData lazyStateHistory)
    {
        var stateType = FindType(lazyStateHistory.StateTypeName);

        if (!stateType.CheckAny())
        {
            throw new Exception("Состояние не найдено");
        }

        return new StateHistory<T>(
            stateType,
            lazyStateHistory.MenuTypeName.CheckAny()
                ? Activator.CreateInstance(FindType(lazyStateHistory.MenuTypeName)) as IMenuButton<T>
                : null,
            lazyStateHistory.IsInlineState
            );

        Type FindType(string name)
        {
            return Array.Find(
                stateFactorySettings
                   .Assembly
                   .GetTypes(),
                z => z.Name == name
                );
        }
    }

    private StateHistory<T> ConvertStateFactoryData(StateFactoryData stateFactoryData = null)
        => stateFactoryData.CheckAny()
            ? Convert(stateFactoryData)
            : CreateContextFunc();
}