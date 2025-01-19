#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory
{
    private StateHistory CreateContextFunc()
    {
        var state = stateFactoryDataCollection.Value
                                              .FirstOrDefault(
                                                   q => q.CommandsTypes.CheckAny()
                                                        && q.CommandsTypes.Any(x => x.In(CommandTypesConstant.StartCommand))
                                                   );

        return Convert(state);
    }

    private StateHistory Convert(StateFactoryData? stateData)
    {
        if (stateData.IsNull())
        {
            throw new("Состояние не найдено");
        }

        var stateType = FindType(stateData!.StateTypeName);

        if (stateType.IsNull())
        {
            throw new("Состояние не найдено");
        }

        return new(
            stateType!,
            stateData.MenuTypeName.IsNotNull()
                ? FindType(stateData.MenuTypeName)
                : null,
            stateData.IsInlineState
            );

        Type? FindType(string name)
        {
            return Array.Find(
                stateFactorySettings.Value
                                    .Assembly
                                    .GetTypes(),
                z => z.Name == name
                );
        }
    }

    private StateHistory ConvertStateFactoryData(StateFactoryData? stateFactoryData = null)
        => stateFactoryData.IsNotNull()
            ? Convert(stateFactoryData)
            : CreateContextFunc();
}