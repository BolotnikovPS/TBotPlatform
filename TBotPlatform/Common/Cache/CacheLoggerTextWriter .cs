using Microsoft.Extensions.Logging;
using System.Text;
using TBotPlatform.Extension;

namespace TBotPlatform.Common.Cache;

internal class CacheLoggerTextWriter(ILogger logger) : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(string value)
    {
        if (value.CheckAny())
        {
            return;
        }

        logger.LogDebug(value);
    }

    public override void Write(char value)
    {
        if (value.IsNotDefault())
        {
            return;
        }

        logger.LogDebug(value.ToString());
    }
}
