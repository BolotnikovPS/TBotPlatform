using TBotPlatform.Contracts.Bots.Constant;

namespace TBotPlatform.Contracts.Bots.Exceptions;

public class MediaGroupCountException(int count)
    : ArgumentException($"An album must contain from {MediaGroupConstant.MinCount} to {MediaGroupConstant.MaxCount} files, but {count} was passed.");
