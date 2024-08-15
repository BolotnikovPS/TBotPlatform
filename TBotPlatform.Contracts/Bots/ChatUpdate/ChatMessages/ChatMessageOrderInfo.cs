#nullable enable
namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMessageOrderInfo(
    string? name,
    string? phoneNumber,
    string? email,
    ChatMessageShippingAddress? shippingAddressOrNull
    )
{
    /// <summary>
    /// Optional. User name
    /// </summary>
    public string Name { get; } = name ?? "";

    /// <summary>
    /// Optional. User's phone number
    /// </summary>
    public string PhoneNumber { get; } = phoneNumber ?? "";

    /// <summary>
    /// Optional. User email
    /// </summary>
    public string Email { get; } = email ?? "";

    /// <summary>
    /// Optional. User shipping address
    /// </summary>
    public ChatMessageShippingAddress? ShippingAddressOrNull { get; } = shippingAddressOrNull;
}