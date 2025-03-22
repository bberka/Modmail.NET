using System.Reflection;
using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Extensions;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Modmail.NET;
using Modmail.NET.Abstract;
using Modmail.NET.Commands;
using Modmail.NET.Commands.Slash;
using Modmail.NET.Database;
using Modmail.NET.Database.Triggers;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Teams;
using Modmail.NET.Jobs;
using Modmail.NET.Language;
using Modmail.NET.Pipeline;
using Modmail.NET.Queues;
using Modmail.NET.Static;
using Modmail.NET.Utils;
using Modmail.NET.Web.Blazor.Components;
using Modmail.NET.Web.Blazor.Providers;
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
builder.Host.UseSerilog(aspNetCoreLogger);
builder.Logging.AddSerilog(appLogger);

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
  options.Name = Const.ThemeCookieName; // The name of the cookie
  options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});

builder.Services.AddRadzenComponents();

builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

#endregion

#region ASP NET CORE

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

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

var assembly = typeof(ModmailBotProjectMarker).Assembly; // Or specify a different assembly

var jobDefinitions = assembly.GetTypes()
                             .Where(t => typeof(IRecurringJobDefinition).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
                             .ToList();

foreach (var jobDefinitionType in jobDefinitions) builder.Services.AddSingleton(jobDefinitionType);

builder.Services.AddHangfireServer();

#endregion

#region MEDIATR

builder.Services.AddMediatR(x => {
  x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), typeof(ModmailBotProjectMarker).Assembly);
  x.AddOpenBehavior(typeof(LoggerPipelineBehavior<,>));
  x.AddOpenBehavior(typeof(ValidationBehavior<,>));
  x.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
  x.AddOpenBehavior(typeof(RetryPipelineBehavior<,>));
  x.Lifetime = ServiceLifetime.Scoped;
});

#endregion

#region FLUENT VALIDATION

builder.Services.AddValidatorsFromAssemblyContaining(typeof(ModmailBotProjectMarker));

#endregion

#region DISCORD CLIENT

builder.Services.AddDiscordClient(botConfig.GetValue<string>("BotToken") ?? throw new Exception("BotToken is empty"),
                                  DiscordIntents.MessageContents
                                  | DiscordIntents.Guilds
                                  | DiscordIntents.GuildMessages
                                  | DiscordIntents.GuildMembers
                                  | DiscordIntents.DirectMessages
                                  | DiscordIntents.GuildMessages
                                  | DiscordIntents.DirectMessageReactions);

builder.Services.AddCommandsExtension((_, extension) => {
  extension.AddCommands<ModmailCommands>();
  //TODO: Enable disable commands option
  extension.AddCommands<BlacklistSlashCommands>();
  extension.AddCommands<TicketSlashCommands>();

  extension.AddChecks(typeof(ModmailBotProjectMarker).Assembly);
  TextCommandProcessor textCommandProcessor = new(new TextCommandConfiguration {
    PrefixResolver = new DefaultPrefixResolver(true, botConfig.GetValue<string>("BotPrefix") ?? throw new Exception("BotPrefix is null")).ResolvePrefixAsync,
    IgnoreBots = true,
    SuppressMissingMessageContentIntentWarning = false,
    CommandNameComparer = StringComparer.InvariantCultureIgnoreCase
  });

  extension.AddProcessor(textCommandProcessor);

  extension.CommandErrored += (ext, args) => {
    Log.Error(args.Exception, "Error executing command {Command} {@Args}", args.Context.Command.FullName, args.Context.Arguments);
    return Task.CompletedTask;
  };
});


builder.Services.ConfigureEventHandlers(eventHandlingBuilder => {
  eventHandlingBuilder.HandleMessageCreated(ModmailEventHandlers.OnMessageCreated);
  eventHandlingBuilder.HandleChannelDeleted(ModmailEventHandlers.OnChannelDeleted);

  eventHandlingBuilder.HandleInteractionCreated(ModmailEventHandlers.InteractionCreated);
  eventHandlingBuilder.HandleComponentInteractionCreated(ModmailEventHandlers.ComponentInteractionCreated);
  eventHandlingBuilder.HandleModalSubmitted(ModmailEventHandlers.ModalSubmitted);

  eventHandlingBuilder.HandleGuildMemberAdded(ModmailEventHandlers.OnGuildMemberAdded);
  eventHandlingBuilder.HandleGuildMemberRemoved(ModmailEventHandlers.OnGuildMemberRemoved);
  eventHandlingBuilder.HandleGuildBanAdded(ModmailEventHandlers.OnGuildBanAdded);
  eventHandlingBuilder.HandleGuildBanAdded(ModmailEventHandlers.OnGuildBanAdded);
  eventHandlingBuilder.HandleGuildBanRemoved(ModmailEventHandlers.OnGuildBanRemoved);

  eventHandlingBuilder.HandleUserUpdated(ModmailEventHandlers.OnUserUpdated);
  eventHandlingBuilder.HandleUserSettingsUpdated(ModmailEventHandlers.OnUserSettingsUpdated);

  eventHandlingBuilder.HandleMessageReactionAdded(ModmailEventHandlers.OnMessageReactionAdded);
  eventHandlingBuilder.HandleMessageDeleted(ModmailEventHandlers.OnMessageDeleted);
  eventHandlingBuilder.HandleMessageUpdated(ModmailEventHandlers.OnMessageUpdated);
});

#endregion


#region AUTH

builder.Services.AddAuthentication(options => {
         options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
         options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
         options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
       })
       .AddCookie(x => {
         x.LoginPath = "/auth/login";
         x.LogoutPath = "/auth/logout";
         x.AccessDeniedPath = "/accessdenied";
         x.ExpireTimeSpan = TimeSpan.FromDays(1);
         x.SlidingExpiration = true;
         x.Cookie.HttpOnly = true; // Prevent client-side access
         x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
         x.Cookie.SameSite = SameSiteMode.Lax;
       })
       .AddDiscord(options => {
         options.ClientId = botConfig.GetValue<string>("BotClientId") ?? throw new Exception("BotClientId is empty");
         options.ClientSecret = botConfig.GetValue<string>("BotClientSecret") ?? throw new Exception("BotClientSecret is empty");
         options.CallbackPath = "/auth/callback"; // Redirect URI
         options.SaveTokens = true; // Save tokens for later use
         options.Scope.Add("identify"); // Fetch user details
         options.Scope.Add("guilds"); // Fetch guilds (optional, for roles)
         options.AccessDeniedPath = "/result?m=AccessDenied";
         options.CorrelationCookie.SameSite = SameSiteMode.Lax;
         options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
         options.Events.OnCreatingTicket += async context => {
           if (context.Principal?.Identity?.IsAuthenticated != true) {
             context.Fail("Invalid Token");
             return;
           }

           var identity = (ClaimsIdentity)context.Principal.Identity;
           identity.AddClaim(new Claim("access_token", context.AccessToken ?? string.Empty));
           identity.AddClaim(new Claim("refresh_token", context.RefreshToken ?? string.Empty));
           identity.AddClaim(new Claim("token_type", context.TokenType ?? string.Empty));

           var identifier = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           if (identifier is null) {
             context.Fail("Invalid Token");
             return;
           }

           var userId = ulong.Parse(identifier);

           var scope = context.HttpContext.RequestServices.CreateScope();
           var sender = scope.ServiceProvider.GetRequiredService<ISender>();

           var bot = scope.ServiceProvider.GetRequiredService<ModmailBot>();
           try {
             var guild = await bot.GetMainGuildAsync();
             var discordMember = await guild.GetMemberAsync(userId);
             var roles = discordMember.Roles.Select(x => x.Id).ToList();
             var permission = await sender.Send(new GetTeamPermissionLevelQuery(userId, roles));
             if (permission is null) {
               context.Fail("Not member of any team");
               return;
             }

             identity.AddClaim(new Claim(ClaimTypes.Role, permission.ToString() ?? throw new NullReferenceException()));
             Log.Information("Discord.OAuth access granted {UserId} {UserName} {Permission}", userId, discordMember.DisplayName, permission.ToString());
             context.Success();
           }
           catch (BotExceptionBase ex) {
             context.Fail(ex.TitleMessage + " : " + ex.ContentMessage);
           }
           catch (Exception ex) {
             context.Fail(ex);
           }
         };
         options.Events.OnRemoteFailure += context => {
           context.Response.Redirect("/result?m=RemoteAuthFailure");
           return Task.CompletedTask;
         };
         options.Events.OnAccessDenied += context => {
           context.Response.Redirect("/result?m=AccessDenied");
           return Task.CompletedTask;
         };
       });


builder.Services.AddAuthorizationBuilder()
       .AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("Role", "Admin"));


builder.Services.AddScoped<AuthenticationStateProvider, DiscordAuthenticationStateProvider>();

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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions() {
  Authorization = [
    new HangfireAuthorizationProvider(app.Services)
  ]
});

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

Log.Information("Starting Modmail.NET v{Version}", UtilVersion.GetReadableProductVersion());
app.Run();