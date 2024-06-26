namespace TBotPlatform.Contracts.Attributes;

public class StateInlineActivatorAttribute : StateActivatorBaseAttribute
{
    public new bool IsInlineState { get; private set; } = true;
    public new Type MenuType { get; private set; } = null;
}