using System.Reflection;
using DSharpPlus.Exceptions;
using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Modmail.NET;
using Modmail.NET.Abstract;
using Modmail.NET.Database;
using Modmail.NET.Database.Triggers;
using Modmail.NET.Jobs;
using Modmail.NET.Language;
using Modmail.NET.Pipeline;
using Modmail.NET.Queues;
using Modmail.NET.Static;
using Modmail.NET.Web.Blazor.Components;
using Modmail.NET.Web.Blazor.Services;
using Radzen;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Settings.Configuration;

var builder = WebApplication.CreateBuilder(args);

#region LOGGER

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

var efCoreLogger = new LoggerConfiguration()
                   .ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions {
                     SectionName = "EfCoreLogger"
                   })
                   .CreateLogger();

Log.Logger = appLogger;
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(aspNetCoreLogger);

#endregion


#region BASE CONFIGURATION

var botConfig = builder.Configuration.GetSection("Bot");

AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
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

#endregion

#region UI

builder.Services.AddRadzenCookieThemeService(options => {
  options.Name = Const.THEME_COOKIE_NAME; // The name of the cookie
  options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});

builder.Services.AddRadzenComponents();

builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

#endregion

#region ASP NET CORE

builder.Services.AddMemoryCache();

#endregion

#region DI

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

#endregion

#region HANGFIRE

builder.Services.AddHangfire(configuration => configuration
                                              .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                              .UseSimpleAssemblyNameTypeSerializer()
                                              .UseRecommendedSerializerSettings()
                                              .UseSqlServerStorage(botConfig.GetValue<string>("DbConnectionString")));

var assembly = typeof(TicketTimeoutJob).Assembly; // Or specify a different assembly

var jobDefinitions = assembly.GetTypes()
                             .Where(t => typeof(IRecurringJobDefinition).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
                             .ToList();

foreach (var jobDefinitionType in jobDefinitions) builder.Services.AddSingleton(jobDefinitionType);

builder.Services.AddHangfireServer();

#endregion

#region MEDIATR

builder.Services.AddMediatR(x => {
  x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), typeof(ModmailBot).Assembly);
  x.AddOpenBehavior(typeof(LoggerPipelineBehavior<,>));
  x.AddOpenBehavior(typeof(ValidationBehavior<,>));
  x.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
  x.AddOpenBehavior(typeof(RetryPipelineBehavior<,>));
  x.Lifetime = ServiceLifetime.Scoped;
});

#endregion

#region FLUENT VALIDATION

builder.Services.AddValidatorsFromAssemblyContaining(typeof(ModmailBot));

#endregion


var app = builder.Build();
ServiceLocator.Initialize(app.Services);

#region DEV

if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error", true);
  app.UseHsts();
}

#endregion

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// app.UseAuthentication();
// app.UseAuthorization();

app.UseHangfireDashboard();


app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();


#region INIT DATABASE

//Using statement must be used with brackets, otherwise they will not be disposed unless manually done so.
using (var scope = app.Services.CreateScope()) {
  await using (var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>()) {
    try {
      await dbContext.Database.MigrateAsync();
      Log.Information("Database migration completed!");
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to setup server: Database migration failed");
      throw;
    }
  }
}

#endregion

#region REGISTER HANGFIRE JOBS

using (var scope = app.Services.CreateScope()) {
  var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
  var registeredJobDefinitions = scope.ServiceProvider.GetServices<IRecurringJobDefinition>();
  foreach (var jobDefinition in registeredJobDefinitions) jobDefinition.RegisterRecurringJob(recurringJobManager);
}

#endregion

Log.Information("Modmail.NET UI started!");
app.Run();