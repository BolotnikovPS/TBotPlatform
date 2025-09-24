using TBotPlatform.Extension;
using Telegram.Bot;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task AnswerCallbackQuery(
        string text,
        bool showAlert,
        string url,
        int? cacheTime,
        CancellationToken cancellationToken
        )
    {
        if (ChatUpdate.IsNull())
        {
            return Task.CompletedTask;
        }

        ChatIdValidOrThrow();
        CallbackQueryValidOrThrow(ChatUpdate!.CallbackQuery);

        return telegramContext.AnswerCallbackQuery(
            ChatUpdate.CallbackQuery!.Id,
            text,
            showAlert,
            url,
            cacheTime,
            cancellationToken
            );
    }
}