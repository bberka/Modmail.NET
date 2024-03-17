using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Commands;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Events;
using Modmail.NET.Static;
using Ninject;
using Serilog;
using Serilog.Extensions.Logging;

namespace Modmail.NET;

public class ModmailBot
{
  private static ModmailBot? _instance;


  private ModmailBot() {
    _ = MMConfig.This; // Initialize the environment container
    UtilLogConfig.Configure();
    if (MMConfig.This.Environment == EnvironmentType.Development)
      Log.Information("Running in development mode");

    var kernel = new StandardKernel(new MmKernel());
    ServiceLocator.Initialize(kernel);
  }

  public static ModmailBot This {
    get {
      _instance ??= new ModmailBot();
      return _instance;
    }
  }

  public DiscordClient Client { get; private set; }
  // public ServiceProvider Services { get; private set; }

  public async Task StartAsync() {
    // Start the bot
    Client = new DiscordClient(new DiscordConfiguration {
      Token = MMConfig.This.BotToken,
      AutoReconnect = true,
      TokenType = TokenType.Bot,
      Intents = DiscordIntents.All,
      HttpTimeout = TimeSpan.FromSeconds(10),
      LogUnknownEvents = false,
      LoggerFactory = new SerilogLoggerFactory(Log.Logger)
    });
    Client.Heartbeated += BaseEvents.OnHeartbeat;
    Client.Ready += BaseEvents.OnReady;
    Client.ClientErrored += BaseEvents.OnClientError;
    Client.SocketErrored += BaseEvents.OnSocketError;

    Client.MessageCreated += MessageEventHandlers.OnMessageCreated;
    Client.ChannelDeleted += ChannelEventHandlers.OnChannelDeleted;
    
    Client.InteractionCreated += InteractionEventHandlers.InteractionCreated;
    Client.ComponentInteractionCreated += InteractionEventHandlers.ComponentInteractionCreated;
     

    var slash = Client.UseSlashCommands();
    slash.RegisterCommands<ModmailSlashCommands>();
    slash.RegisterCommands<TicketSlashCommands>();
    slash.RegisterCommands<TeamSlashCommands>();
    slash.RegisterCommands<TagSlashCommands>();

    await Client.ConnectAsync();

    await SetupDatabase();

    await Task.Delay(-1);
  }

  private async Task StopAsync() {
    await Client.DisconnectAsync();
  }


  public async Task SetupDatabase() {
    await using var dbContext = new ModmailDbContext();
    try {
      await dbContext.Database.EnsureCreatedAsync();
      await dbContext.Database.MigrateAsync();
      Log.Information("Database migration completed!");
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to setup server: Database migration failed");
      throw;
    }
  }
}