namespace TBotPlatform.Contracts.Attributes;

public class AdminStateActivatorAttribute(Type menuType) : StateActivatorBaseAttribute(isInlineState: false, menuType, true);
