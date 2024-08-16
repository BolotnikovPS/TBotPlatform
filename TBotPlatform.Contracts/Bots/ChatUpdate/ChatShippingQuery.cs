using TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate;

public class ChatShippingQuery(string id, TelegramUser fromUser, string invoicePayload, ChatShippingAddress shippingAddress)
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
    /// Bot specified invoice payload
    /// </summary>
    public string InvoicePayload { get; } = invoicePayload;

    /// <summary>
    /// User specified shipping address
    /// </summary>
    public ChatShippingAddress ShippingAddress { get; } = shippingAddress;
}