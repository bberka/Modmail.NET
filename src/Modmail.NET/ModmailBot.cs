using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Cache;
using Modmail.NET.Commands;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Events;
using Modmail.NET.Static;
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
  }

  public static ModmailBot This {
    get {
      _instance ??= new ModmailBot();
      return _instance;
    }
  }

  public DiscordClient Client { get; private set; }

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
    Client.MessageDeleted += MessageEventHandlers.OnMessageDeleted;
    Client.MessageUpdated += MessageEventHandlers.OnMessageUpdated;
    Client.ChannelDeleted += ChannelEventHandlers.OnChannelDeleted;

    var commandsConfig = new CommandsNextConfiguration {
      StringPrefixes = new[] { MMConfig.This.BotPrefix },
      EnableDms = true,
      EnableMentionPrefix = true,
      DmHelp = false,
      CaseSensitive = false,
      IgnoreExtraArguments = true
    };
    var slash = Client.UseSlashCommands();
    var commands = Client.UseCommandsNext(commandsConfig);
    commands.RegisterCommands<AdminCommands>();


    await Client.ConnectAsync();

    await TrySetupServer();

    await Task.Delay(-1);
  }

  private async Task StopAsync() {
    await Client.DisconnectAsync();
  }


  public async Task<DiscordGuild> GetMainGuildAsync() {
    const int cacheTimeSeconds = 60;
    const string cacheKey = "MainGuild";
    var guild = (DiscordGuild?)DiscordDataCache.This.Get(cacheKey);
    if (guild is not null) return guild;
    guild = await Client.GetGuildAsync(MMConfig.This.MainServerId);
    if (guild is null) throw new Exception("Failed to get main guild");
    DiscordDataCache.This.Set(cacheKey, guild, TimeSpan.FromSeconds(cacheTimeSeconds));
    return guild;
  }

  public async Task<DiscordChannel> GetLogChannelAsync() {
    const int cacheTimeSeconds = 60;
    const string cacheKey = "LogChannel";
    var logChannel = (DiscordChannel?)DiscordDataCache.This.Get(cacheKey);
    if (logChannel is not null) return logChannel;
    var ctx = new ModmailDbContext();
    var option = await ctx.GetOptionAsync(MMConfig.This.MainServerId);
    if (option is null) throw new Exception("TicketOption not found for guild");
    await ctx.DisposeAsync();
    // var guild = await GetMainGuildAsync();
    logChannel = (DiscordChannel?)DiscordDataCache.This.Get(cacheKey);
    if (logChannel is null) throw new Exception("LogChannel not found in cache");
    DiscordDataCache.This.Set(cacheKey, logChannel, TimeSpan.FromSeconds(cacheTimeSeconds));
    return logChannel;
  }


  public async Task TrySetupServer() {
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

    var existingMmOption = await dbContext.TicketOptions.FirstOrDefaultAsync(x => x.GuildId == MMConfig.This.MainServerId);
    if (existingMmOption is not null) {
      Log.Information("Server already setup!");
      return;
    }

    var mainGuild = await GetMainGuildAsync();

    //create a channel 
    var category = await mainGuild.CreateChannelCategoryAsync("Modmail");

    //create a log channel
    var logChannel = await mainGuild.CreateTextChannelAsync("modmail-logs", category);

    var mmOption = new TicketOption {
      CategoryId = category.Id,
      GuildId = mainGuild.Id,
      LogChannelId = logChannel.Id,
      IsListenPrivateMessages = true
    };
    dbContext.TicketOptions.Add(mmOption);
    await dbContext.SaveChangesAsync();

    Log.Information("Server setup completed!");
  }
}