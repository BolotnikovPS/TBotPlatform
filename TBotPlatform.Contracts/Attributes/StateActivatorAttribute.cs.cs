namespace TBotPlatform.Contracts.Attributes;

public class StateActivatorAttribute : StateActivatorBaseAttribute
{
    public new bool IsInlineState { get; private set; } = false;
}