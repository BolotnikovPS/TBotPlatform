using TBotPlatform.Contracts.Bots.Constant;

namespace TBotPlatform.Contracts.Bots.Exceptions;

public class MediaGroupCountException(int count)
    : ArgumentException($"Альбом должен содержать от {MediaGroupConstant.MinCount} до {MediaGroupConstant.MaxCount} файлов, передано {count}.");
