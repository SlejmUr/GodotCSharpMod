using Serilog;

namespace Game.csharp
{
    internal class GameLog
    {
        public static ILogger logger = null;

        public static void CreateNew()
        {
            logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            logger.Information("Application started!");
        }

        public static void Close()
        {
            if (logger != null)
            {
                logger.Information("Application closed!");
                Log.CloseAndFlush();
                logger = null;
            }
        }
    }
}
