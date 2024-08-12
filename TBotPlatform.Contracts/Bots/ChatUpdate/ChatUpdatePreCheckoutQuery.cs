#nullable enable
using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatUpdatePreCheckoutQuery(
    string id,
    TelegramUser fromUser,
    string currency,
    int totalAmount,
    string invoicePayload,
    string? shippingOptionId,
    ChatMessageOrderInfo? orderInfoOrNull
    )
{
    /// <summary>
    /// Unique query identifier
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// User who sent the query
    /// </summary>
    public TelegramUser FromUser { get; } = fromUser;

    /// <summary>
    /// Three-letter ISO 4217
    /// <a href="https://core.telegram.org/bots/payments#supported-currencies">currency</a> code
    /// </summary>
    public string Currency { get; } = currency;

    /// <summary>
    /// Total price in the <i>smallest units</i> of the
    /// <a href="https://core.telegram.org/bots/payments#supported-currencies">currency</a>
    /// (integer, <b>not</b> float/double).
    /// <para>
    /// For example, for a price of <c>US$ 1.45</c> pass <c>amount = 145</c>. See the <i>exp</i> parameter in
    /// <a href="https://core.telegram.org/bots/payments/currencies.json">currencies.json</a>, it shows the
    /// number of digits past the decimal point for each currency (2 for the majority of currencies).
    /// </para>
    /// </summary>
    public int TotalAmount { get; } = totalAmount;

    /// <summary>
    /// Bot specified invoice payload
    /// </summary>
    public string InvoicePayload { get; } = invoicePayload;

    /// <summary>
    /// Optional. Identifier of the shipping option chosen by the user
    /// </summary>
    public string? ShippingOptionId { get; } = shippingOptionId;

    /// <summary>
    /// Optional. Order info provided by the user
    /// </summary>
    public ChatMessageOrderInfo? OrderInfoOrNull { get; } = orderInfoOrNull;
}