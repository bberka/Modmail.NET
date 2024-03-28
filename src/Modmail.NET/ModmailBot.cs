using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Aspects;
using Modmail.NET.Commands;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Events;
using Modmail.NET.Exceptions;
using Modmail.NET.Manager;
using Modmail.NET.Static;
using Modmail.NET.Utils;
using Ninject;
using Serilog;
using Serilog.Extensions.Logging;

namespace Modmail.NET;

public class ModmailBot
{
  private static ModmailBot? _instance;


  private ModmailBot() {
    _ = BotConfig.This; // Initialize the environment container
    UtilLogConfig.Configure();
    if (BotConfig.This.Environment == EnvironmentType.Development)
      Log.Information("Running in development mode");

    var kernel = new StandardKernel(new MmKernel());
    ServiceLocator.Initialize(kernel);

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
    Log.Information("Starting Modmail.NET v{Version}", UtilVersion.GetVersion());
    AutoStartMgr.SaveAutoStart();
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
      Token = BotConfig.This.BotToken,
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
    Client.ModalSubmitted += ModalSubmitted.Handle;

    //FOR USER DATA UPDATE ONLY
    Client.GuildMemberAdded += OnGuildMemberAdded.Handle;
    Client.GuildMemberRemoved += OnGuildMemberRemoved.Handle;
    Client.GuildBanAdded += OnGuildBanAdded.Handle;
    Client.GuildBanRemoved += OnGuildBanRemoved.Handle;
    Client.MessageAcknowledged += OnMessageAcknowledged.Handle;
    Client.UserUpdated += OnUserUpdated.Handle;
    Client.UserSettingsUpdated += OnUserSettingsUpdated.Handle;
    Client.ScheduledGuildEventUserAdded += OnScheduledGuildEventUserAdded.Handle;
    Client.ScheduledGuildEventUserRemoved += OnScheduledGuildEventUserRemoved.Handle;
    Client.MessageReactionAdded += OnMessageReactionAdded.Handle;
    Client.MessageReactionRemoved += OnMessageReactionRemoved.Handle;
    Client.MessageReactionRemovedEmoji += OnMessageReactionRemovedEmoji.Handle;
    Client.MessageReactionsCleared += OnMessageReactionsCleared.Handle;
    Client.MessageDeleted += OnMessageDeleted.Handle;
    Client.MessageUpdated += OnMessageUpdated.Handle;
    Client.ThreadCreated += OnThreadCreated.Handle;


    var slash = Client.UseSlashCommands();
    slash.RegisterCommands<ModmailSlashCommands>();
    slash.RegisterCommands<TicketSlashCommands>();
    slash.RegisterCommands<TeamSlashCommands>();
    slash.RegisterCommands<BlacklistSlashCommands>();
    slash.RegisterCommands<TicketTypeSlashCommands>();

    await Client.ConnectAsync();

    await SetupDatabase();

    await Task.Delay(5);

    await Client.UpdateStatusAsync(Const.DISCORD_ACTIVITY);

    await Task.Delay(-1);
  }

  private async Task StopAsync() {
    await Client.DisconnectAsync();
  }


  public async Task SetupDatabase() {
    await using var dbContext = new ModmailDbContext();
    try {
      await dbContext.Database.MigrateAsync();
      Log.Information("Database migration completed!");
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to setup server: Database migration failed");
      throw;
    }

    await DiscordUserInfo.AddOrUpdateAsync(Client.CurrentUser);
  }


  public async Task<DiscordMember?> GetMemberFromAnyGuildAsync(ulong userId) {
    foreach (var guild in Client.Guilds) {
      try {
        var member = await guild.Value.GetMemberAsync(userId, false);
        if (member != null) {
          await DiscordUserInfo.AddOrUpdateAsync(member);
          return member;
        }
      }
      catch (Exception ex) {
        Log.Error(ex, "Failed to get member from guild {GuildId} for user {UserId}", guild.Key, userId);
      }
    }

    return null;
  }

  [CacheAspect(CacheSeconds = 60)]
  public async Task<DiscordGuild> GetMainGuildAsync() {
    var guildId = BotConfig.This.MainServerId;
    var guild = await Client.GetGuildAsync(guildId);
    if (guild == null) {
      Log.Error("Main guild not found: {GuildId}", guildId);
      throw new MainGuildNotFoundException();
    }

    var guildOption = await GuildOption.GetAsync();

    if (guildOption is not null) {
      guildOption.Name = guild.Name;
      guildOption.IconUrl = guild.IconUrl;
      guildOption.BannerUrl = guild.BannerUrl;
      await guildOption.UpdateAsync();
      await DiscordUserInfo.AddOrUpdateAsync(guild.Owner);
    }

    return guild;
  }

  [CacheAspect(CacheSeconds = 300)]
  public async Task<DiscordChannel> GetLogChannelAsync() {
    var guild = await GetMainGuildAsync();
    var option = await GuildOption.GetAsync();
    if (option is null) {
      throw new ServerIsNotSetupException();
    }

    var logChannel = guild.GetChannel(option.LogChannelId);

    if (logChannel is null) {
      throw new LogChannelNotFoundException();
    }

    return logChannel;
  }
}