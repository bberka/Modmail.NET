using DSharpPlus.Exceptions;
using Serilog;
using Serilog.Settings.Configuration;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class AspNetDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    var appLogger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions {
                      SectionName = "AppLogger"
                    })
                    .CreateLogger();

    var aspNetCoreLogger = new LoggerConfiguration()
                           .ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions {
                             SectionName = "AspNetCoreLogger"
                           })
                           .CreateLogger();

   

    Log.Logger = appLogger;
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog(aspNetCoreLogger);
    builder.Logging.AddSerilog(appLogger);
    
    builder.Services.AddKeyedSingleton("AppLogger", appLogger);
    builder.Services.AddKeyedSingleton("AspNetCoreLogger", aspNetCoreLogger);

    
    AppDomain.CurrentDomain.UnhandledException += (_, args) => {
      var isDiscordException = args.ExceptionObject is DiscordException;
      if (isDiscordException) {
        var casted = (DiscordException)args.ExceptionObject;
        Log.Error(casted, "Unhandled Discord exception {JsonMessage} {@Data}", casted.JsonMessage, casted.Data);
      }
      else {
        Log.Error((Exception)args.ExceptionObject, "Unhandled exception");
      }
    };

    builder.Host.UseDefaultServiceProvider(x => {
      x.ValidateOnBuild = true;
      x.ValidateScopes = true;
    });

    builder.Services.AddMemoryCache();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers();
  }
}