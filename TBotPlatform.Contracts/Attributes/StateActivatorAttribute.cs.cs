namespace TBotPlatform.Contracts.Attributes;

public class StateActivatorAttribute(Type menuType) : StateActivatorBaseAttribute(isInlineState: false, menuType);