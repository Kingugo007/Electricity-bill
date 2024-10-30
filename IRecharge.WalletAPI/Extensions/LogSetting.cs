using Serilog;
using Serilog.Exceptions;

namespace IRecharge.WalletAPI.Extensions
{
    public class LogSetting
    {
        public static void SetupLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: ".\\Logs\\Wallet-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();
        }
    }
}
