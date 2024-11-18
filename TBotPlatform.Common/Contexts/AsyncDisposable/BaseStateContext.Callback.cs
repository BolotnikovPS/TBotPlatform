namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class BaseStateContext
{
    public Task AnswerCallbackQueryAsync(
        string text,
        bool showAlert,
        string url,
        int? cacheTime,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();
        CallbackQueryValidOrThrow(ChatUpdate.CallbackQueryOrNull);

        return telegramContext.AnswerCallbackQueryAsync(
            ChatUpdate.CallbackQueryOrNull!.Id,
            text,
            showAlert,
            url,
            cacheTime,
            cancellationToken
            );
    }
}