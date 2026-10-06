#nullable enable
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Payments;

namespace TBotPlatform.Common.Contexts.AsyncDisposable;

internal partial class StateContext
{
    public Task AnswerInlineQuery(
        string inlineQueryId,
        IReadOnlyList<InlineQueryResult> results,
        int? cacheTime,
        string? nextOffset,
        CancellationToken cancellationToken
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inlineQueryId);
        ArgumentNullException.ThrowIfNull(results);

        return telegramContext.AnswerInlineQuery(
            inlineQueryId,
            results,
            cacheTime,
            nextOffset: nextOffset,
            cancellationToken: cancellationToken
            );
    }

    public Task<SentWebAppMessage> AnswerWebAppQuery(
        string webAppQueryId,
        InlineQueryResult result,
        CancellationToken cancellationToken
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(webAppQueryId);
        ArgumentNullException.ThrowIfNull(result);

        return telegramContext.AnswerWebAppQuery(webAppQueryId, result, cancellationToken);
    }

    public Task AnswerPreCheckoutQuery(
        string preCheckoutQueryId,
        string? errorMessage,
        CancellationToken cancellationToken
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(preCheckoutQueryId);

        return telegramContext.AnswerPreCheckoutQuery(preCheckoutQueryId, errorMessage, cancellationToken);
    }

    public Task AnswerShippingQuery(
        string shippingQueryId,
        IReadOnlyList<ShippingOption>? shippingOptions,
        string? errorMessage,
        CancellationToken cancellationToken
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(shippingQueryId);

        return telegramContext.AnswerShippingQuery(shippingQueryId, shippingOptions, errorMessage, cancellationToken);
    }

    public Task<Message> SendInvoice(
        string title,
        string description,
        string payload,
        string currency,
        IReadOnlyList<LabeledPrice> prices,
        string? providerToken,
        string? providerData,
        CancellationToken cancellationToken
        )
    {
        ChatIdValidOrThrow();

        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        ArgumentNullException.ThrowIfNull(prices);

        return telegramContext.SendInvoice(
            chatId,
            title,
            description,
            payload,
            currency,
            prices,
            providerToken: providerToken,
            providerData: providerData,
            cancellationToken: cancellationToken
            );
    }
}
