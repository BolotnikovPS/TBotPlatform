#nullable enable
using TBotPlatform.Contracts.Bots;
using TBotPlatform.Contracts.Bots.Constant;
using TBotPlatform.Contracts.Bots.StateFactory;
using TBotPlatform.Extension;
using TBotPlatform.Results;

namespace TBotPlatform.Common.Factories;

internal partial class StateFactory
{
    private ResultT<StateHistory> CreateContextFunc(string botName)
    {
        var state = GetStateFactoryDataCollection(botName)
           .FirstOrDefault(
                q => q.CommandsTypes.CheckAny()
                     && q.CommandsTypes.Any(x => x.In(CommandTypesConstant.StartCommand))
                );

        return Convert(state);
    }

    private ResultT<StateHistory> Convert(StateFactoryData? stateData)
    {
        if (stateData.IsNull())
        {
            return ResultT<StateHistory>.Failure(ErrorResult.NotFound(StateNotFound));
        }

        var stateType = FindType(stateData!.StateTypeName);

        if (stateType.IsNull())
        {
            return ResultT<StateHistory>.Failure(ErrorResult.NotFound(StateNotFound));
        }

        var state = new StateHistory(
            stateType!,
            stateData.MenuTypeName.IsNotNull()
                ? FindType(stateData.MenuTypeName)
                : null,
            stateData.IsInlineState
            );

        return ResultT<StateHistory>.Success(state);

        Type? FindType(string name)
        {
            return Array.Find(assembly.GetTypes(), z => z.Name == name);
        }
    }

    private ResultT<StateHistory> ConvertStateFactoryData(string botName, StateFactoryData? stateFactoryData = null)
        => stateFactoryData.IsNotNull()
            ? Convert(stateFactoryData)
            : CreateContextFunc(botName);
}