using Serilog;
using Serilog.Formatting.Compact;

namespace Modmail.NET.Common;

public static class UtilLogConfig
{
  public static void Configure() {
    const string logPath = "logs\\log.json";
    var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .Enrich.WithThreadId()
                 .MinimumLevel.Is(MMConfig.This.LogLevel)
                 .WriteTo.Console(outputTemplate: template)
                 .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: RollingInterval.Day)
                 .CreateLogger();
  }
}