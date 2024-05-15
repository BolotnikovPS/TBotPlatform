using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramContextHostedServiceSettings
{
    public UpdateType[] UpdateType { get; set; }
}