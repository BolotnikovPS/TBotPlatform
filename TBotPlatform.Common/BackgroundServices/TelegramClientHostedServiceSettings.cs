using Telegram.Bot.Types.Enums;

namespace TBotPlatform.Common.BackgroundServices;

internal class TelegramClientHostedServiceSettings
{
    public UpdateType[] UpdateType { get; set; }
}