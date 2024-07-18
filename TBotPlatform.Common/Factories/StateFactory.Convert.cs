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

    private StateHistory<T> Convert(StateFactoryData stateData)
    {
        var stateType = FindType(stateData.StateTypeName);

        if (stateType.IsNull())
        {
            throw new("Состояние не найдено");
        }

        return new(
            stateType,
            stateData.MenuTypeName.CheckAny()
                ? Activator.CreateInstance(FindType(stateData.MenuTypeName)) as IMenuButton<T>
                : null,
            stateData.IsInlineState
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
        => stateFactoryData.IsNotNull()
            ? Convert(stateFactoryData)
            : CreateContextFunc();
}