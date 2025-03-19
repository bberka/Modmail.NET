using System.Reflection;
using DSharpPlus.Exceptions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Modmail.NET;
using Modmail.NET.Abstract;
using Modmail.NET.Database;
using Modmail.NET.Jobs;
using Modmail.NET.Language;
using Modmail.NET.Utils;
using Modmail.NET.Web.Blazor.Components;
using Modmail.NET.Web.Blazor.Services;
using Modmail.NET.Web.Shared;
using Radzen;
using Serilog;
using MemoryCache = System.Runtime.Caching.MemoryCache;

var builder = WebApplication.CreateBuilder(args);

UtilLogConfig.Configure();

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

// Add services to the container.
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddHangfire(configuration => configuration
                                              .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                              .UseSimpleAssemblyNameTypeSerializer()
                                              .UseRecommendedSerializerSettings()
                                              .UseSqlServerStorage(BotConfig.This.DbConnectionString));

var assembly = typeof(TicketTimeoutJob).Assembly; // Or specify a different assembly

var jobDefinitions = assembly.GetTypes()
                             .Where(t => typeof(IRecurringJobDefinition).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                             .ToList();

foreach (var jobDefinitionType in jobDefinitions) {
  builder.Services.AddSingleton(typeof(IRecurringJobDefinition), jobDefinitionType);
}

builder.Services.AddHangfireServer();

builder.Services.AddSingleton(LangData.This);
builder.Services.AddSingleton(BotConfig.This);
builder.Services.AddSingleton(ModmailBot.This);
builder.Services.AddHostedService<ModmailHostedService>();
builder.Services.AddDbContextFactory<ModmailDbContext>();
builder.Services.AddDbContext<ModmailDbContext>();

builder.Services.AddRadzenCookieThemeService(options => {
  options.Name = WebSharedConstants.THEME_COOKIE_NAME; // The name of the cookie
  options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});

builder.Services.AddRadzenComponents();


builder.Services.AddMemoryCache();
builder.Services.AddDbContext<ModmailDbContext>();


var dbContext = new ModmailDbContext();
try {
  await dbContext.Database.MigrateAsync();
  Log.Information("Database migration completed!");
}
catch (Exception ex) {
  Log.Error(ex, "Failed to setup server: Database migration failed");
  throw;
}
finally {
  dbContext.Dispose();
}

var app = builder.Build();
ServiceLocator.Initialize(app.Services);

if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error", true);
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// app.UseAuthentication();
// app.UseAuthorization();

app.UseHangfireDashboard();


app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope()) {
  var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
  var registeredJobDefinitions = scope.ServiceProvider.GetServices<IRecurringJobDefinition>();
  foreach (var jobDefinition in registeredJobDefinitions) {
    jobDefinition.RegisterRecurringJob(recurringJobManager);
  }
}
app.Run();