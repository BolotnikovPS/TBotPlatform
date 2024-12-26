#nullable enable
namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatOrderInfo(
    string? name,
    string? phoneNumber,
    string? email,
    ChatShippingAddress? shippingAddressOrNull
    )
{
    /// <summary>
    /// Optional. UserName
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
    public ChatShippingAddress? ShippingAddressOrNull { get; } = shippingAddressOrNull;
}