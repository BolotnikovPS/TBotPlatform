﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TBotPlatform.Contracts.Bots.Users;

namespace TBotPlatform.Contracts.Bots.ChatUpdate.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum EChatEntityType
{
    None = 0,

    /// <summary>
    /// A mentioned
    /// </summary>
    Mention = 1,

    /// <summary>
    /// A searchable Hashtag
    /// </summary>
    Hashtag,

    /// <summary>
    /// A Bot command
    /// </summary>
    BotCommand,

    /// <summary>
    /// An URL
    /// </summary>
    Url,

    /// <summary>
    /// An email
    /// </summary>
    Email,

    /// <summary>
    /// Bold text
    /// </summary>
    Bold,

    /// <summary>
    /// Italic text
    /// </summary>
    Italic,

    /// <summary>
    /// Monowidth string
    /// </summary>
    Code,

    /// <summary>
    /// Monowidth block
    /// </summary>
    Pre,

    /// <summary>
    /// Clickable text URLs
    /// </summary>
    TextLink,

    /// <summary>
    /// Mentions for a <see cref="TelegramUser"/> without <see cref="TelegramUser.UserName"/>
    /// </summary>
    TextMention,

    /// <summary>
    /// Phone number
    /// </summary>
    PhoneNumber,

    /// <summary>
    /// A cashtag (e.g. $EUR, $USD) - $ followed by the short currency code
    /// </summary>
    Cashtag,

    /// <summary>
    /// Underlined text
    /// </summary>
    Underline,

    /// <summary>
    /// Strikethrough text
    /// </summary>
    Strikethrough,

    /// <summary>
    /// Spoiler message
    /// </summary>
    Spoiler,

    /// <summary>
    /// Inline custom emoji stickers
    /// </summary>
    CustomEmoji,
}