using Telegram.Bot.Types;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Checks that the chat member is an administrator or chat owner
    /// </summary>
    /// <param name="chatMember">Chat member</param>
    /// <returns>true if the member is an administrator or owner</returns>
    public static bool IsAdminOrCreator(this ChatMember? chatMember)
        => chatMember is ChatMemberOwner or ChatMemberAdministrator;

    /// <summary>
    /// Checks that the chat member is still in the chat (not left, not banned)
    /// </summary>
    /// <param name="chatMember">Chat member</param>
    /// <returns>true when the member has not left the chat and is not banned</returns>
    public static bool IsInChat(this ChatMember? chatMember)
        => chatMember switch
        {
            ChatMemberOwner or ChatMemberAdministrator or ChatMemberMember => true,
            ChatMemberRestricted restricted => restricted.IsMember,
            _ => false,
        };
}