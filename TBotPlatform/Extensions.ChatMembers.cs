using Telegram.Bot.Types;

namespace TBotPlatform.Extension;

public static partial class Extensions
{
    /// <summary>
    /// Проверяет, что участник чата является администратором или владельцем чата
    /// </summary>
    /// <param name="chatMember">Участник чата</param>
    /// <returns>true, если участник администратор или владелец</returns>
    public static bool IsAdminOrCreator(this ChatMember? chatMember)
        => chatMember is ChatMemberOwner or ChatMemberAdministrator;

    /// <summary>
    /// Проверяет, что участник находится в чате
    /// </summary>
    /// <param name="chatMember">Участник чата</param>
    /// <returns>true, если участник не покинул чат и не заблокирован</returns>
    public static bool IsInChat(this ChatMember? chatMember)
        => chatMember switch
        {
            ChatMemberOwner or ChatMemberAdministrator or ChatMemberMember => true,
            ChatMemberRestricted restricted => restricted.IsMember,
            _ => false,
        };
}
