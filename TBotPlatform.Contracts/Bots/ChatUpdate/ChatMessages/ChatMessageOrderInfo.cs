#nullable enable
namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMessageOrderInfo(
    string? name,
    string? phoneNumberOrNull,
    string? email,
    ChatMessageShippingAddress? shippingAddress
    )
{
    /// <summary>
    /// Optional. User name
    /// </summary>
    public string? Name { get; } = name;

    /// <summary>
    /// Optional. User's phone number
    /// </summary>
    public string? PhoneNumberOrNull { get; } = phoneNumberOrNull;

    /// <summary>
    /// Optional. User email
    /// </summary>
    public string? Email { get; } = email;

    /// <summary>
    /// Optional. User shipping address
    /// </summary>
    public ChatMessageShippingAddress? ShippingAddress { get; } = shippingAddress;
}