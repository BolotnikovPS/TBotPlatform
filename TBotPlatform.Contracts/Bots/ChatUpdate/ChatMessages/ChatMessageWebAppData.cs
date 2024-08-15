namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatMessageWebAppData(string data, string buttonText)
{
    /// <summary>
    /// The data. Be aware that a bad client can send arbitrary data in this field.
    /// </summary>
    public string Data { get; } = data;

    /// <summary>
    /// Text of the web_app keyboard button, from which the Web App was opened. Be aware that a bad client can
    /// send arbitrary data in this field.
    /// </summary>
    public string ButtonText { get; } = buttonText;
}