namespace TBotPlatform.Contracts.Bots.Markups.Enums;

public enum EInlineMarkupType
{
    None = 0,
    CallbackData,
    Url,
    LoginUrl,
    CallbackGame,
    Payment,
    SwitchInlineQuery,
    SwitchInlineQueryChosenChat,
    SwitchInlineQueryCurrentChat,
    WebApp,
    CopyText,
}