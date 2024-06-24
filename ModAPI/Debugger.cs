using Serilog;

namespace ModAPI
{
    public class Debugger
    {
        public static ILogger? logger = null;

        public static void ParseLogger(ILogger _logger)
        {
            logger = _logger;
        }

        public static void ClearLogger()
        {
            logger = null;
        }
    }
}
