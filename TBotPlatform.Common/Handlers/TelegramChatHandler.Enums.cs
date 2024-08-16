using TBotPlatform.Contracts.Bots.ChatUpdate.Enums;
using TBotPlatform.Extension;
using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.Handlers;

internal partial class TelegramChatHandler
{
    private static EChatEntityType CreateChatMessageEntityType(MessageEntityType? type)
    {
        if (type.IsNull())
        {
            return EChatEntityType.None;
        }

        return type switch
        {
            MessageEntityType.Mention => EChatEntityType.Mention,
            MessageEntityType.Hashtag => EChatEntityType.Hashtag,
            MessageEntityType.BotCommand => EChatEntityType.BotCommand,
            MessageEntityType.Url => EChatEntityType.Url,
            MessageEntityType.Email => EChatEntityType.Email,
            MessageEntityType.Bold => EChatEntityType.Bold,
            MessageEntityType.Italic => EChatEntityType.Italic,
            MessageEntityType.Code => EChatEntityType.Code,
            MessageEntityType.Pre => EChatEntityType.Pre,
            MessageEntityType.TextLink => EChatEntityType.TextLink,
            MessageEntityType.TextMention => EChatEntityType.TextMention,
            MessageEntityType.PhoneNumber => EChatEntityType.PhoneNumber,
            MessageEntityType.Cashtag => EChatEntityType.Cashtag,
            MessageEntityType.Underline => EChatEntityType.Underline,
            MessageEntityType.Strikethrough => EChatEntityType.Strikethrough,
            MessageEntityType.Spoiler => EChatEntityType.Spoiler,
            MessageEntityType.CustomEmoji => EChatEntityType.CustomEmoji,
            _ => EChatEntityType.None,
        };
    }

    private static EChatType CreateChatType(ChatType? type)
    {
        if (type.IsNull())
        {
            return EChatType.None;
        }

        return type switch
        {
            ChatType.Private => EChatType.Private,
            ChatType.Group => EChatType.Group,
            ChatType.Supergroup => EChatType.Supergroup,
            ChatType.Channel => EChatType.Channel,
            ChatType.Sender => EChatType.Sender,
            _ => EChatType.None,
        };
    }
}