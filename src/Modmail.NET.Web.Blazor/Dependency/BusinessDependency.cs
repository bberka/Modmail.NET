using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Triggers;
using Modmail.NET.Language;
using Modmail.NET.Queues;
using Modmail.NET.Static;
using Modmail.NET.Web.Blazor.Services;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Settings.Configuration;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class BusinessDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    var efCoreLogger = new LoggerConfiguration()
                       .ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions {
                         SectionName = "EfCoreLogger"
                       })
                       .CreateLogger();

    var botConfig = builder.Configuration.GetSection("Bot");
    builder.Services.Configure<BotConfig>(botConfig);
    builder.Services.AddSingleton<LangProvider>();
    builder.Services.AddSingleton<ModmailBot>();
    builder.Services.AddSingleton<ModmailEventHandlers>();
    builder.Services.AddSingleton<TicketMessageQueue>();
    builder.Services.AddHostedService<ModmailHostedService>();

    builder.Services.AddDbContextFactory<ModmailDbContext>(options => {
      options.UseSqlServer(botConfig.GetValue<string>("DbConnectionString"));
      options.UseTriggers(triggerOptions => {
        triggerOptions.AddTrigger<RegisterDateTrigger>();
        triggerOptions.AddTrigger<UpdateDateTrigger>();
      });


      if (botConfig.GetValue<bool>("SensitiveDataLogging")
          && builder.Environment.IsDevelopment()
          && botConfig.GetValue<EnvironmentType>("Environment") == EnvironmentType.Development)
        options.EnableSensitiveDataLogging();

      options.UseLoggerFactory(new SerilogLoggerFactory(efCoreLogger));
    });
  }

  public static void Initialize(WebApplication app) {
    using var scope = app.Services.CreateScope();
    using var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
    try {
      dbContext.Database.MigrateAsync();
      Log.Information("Database migration completed!");
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to setup server: Database migration failed");
      throw;
    }
  }
}