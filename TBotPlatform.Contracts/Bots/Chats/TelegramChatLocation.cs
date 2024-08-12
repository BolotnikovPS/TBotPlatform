using TBotPlatform.Contracts.Bots.Locations;

namespace TBotPlatform.Contracts.Bots.Chats;

public class TelegramChatLocation(TelegramLocation location, string address)
{
    /// <summary>
    /// The location to which the supergroup is connected. Can't be a live location.
    /// </summary>
    public TelegramLocation Location { get; } = location;

    /// <summary>
    /// Location address; 1-64 characters, as defined by the chat owner
    /// </summary>
    public string Address { get; } = address;
}