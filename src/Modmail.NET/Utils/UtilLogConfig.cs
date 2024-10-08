﻿using Serilog;
using Serilog.Formatting.Compact;

namespace Modmail.NET.Utils;

public static class UtilLogConfig
{
  public static void Configure() {
    const string logPath = "Logs\\Log_.json";
    var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .Enrich.WithThreadId()
                 .MinimumLevel.Is(BotConfig.This.LogLevel)
                 .Enrich.FromLogContext()
                 .WriteTo.Console(outputTemplate: template)
                 .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: RollingInterval.Day)
                 .CreateLogger();
  }
}