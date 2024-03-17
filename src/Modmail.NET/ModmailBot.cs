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
    Client.Heartbeated += OnHeartbeat.Handle;
    Client.Ready += OnReady.Handle;
    Client.ClientErrored += OnClientError.Handle;
    Client.SocketErrored += OnSocketError.Handle;

    Client.MessageCreated += OnMessageCreated.Handle;
    Client.ChannelDeleted += OnChannelDeleted.Handle;
    
    Client.InteractionCreated += InteractionCreated.Handle;
    Client.ComponentInteractionCreated += ComponentInteractionCreated.Handle;
     

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