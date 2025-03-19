using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Commands;
using Modmail.NET.Commands.Slash;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Events;
using Modmail.NET.Exceptions;
using Modmail.NET.Utils;
using Serilog;
using Serilog.Extensions.Logging;
using NotFoundException = Modmail.NET.Exceptions.NotFoundException;

namespace Modmail.NET;

public class ModmailBot
{
  private readonly BotConfig _config;
  private static ModmailBot? _instance;

  public ModmailBot(BotConfig config) {
    _config = config;



    if (BotConfig.This.Environment == EnvironmentType.Development)
      Log.Information("Running in development mode");



    Log.Information("Starting Modmail.NET v{Version}", UtilVersion.GetVersion());
    //Define the client
    Client = new DiscordClient(new DiscordConfiguration {
      Token = BotConfig.This.BotToken,
      AutoReconnect = true,
      TokenType = TokenType.Bot,
      Intents = DiscordIntents.All,
      HttpTimeout = TimeSpan.FromSeconds(10),
      LogUnknownEvents = false,
      LoggerFactory = new SerilogLoggerFactory(Log.Logger)
    });

    //Define the events
    Client.Heartbeated += OnHeartbeat.Handle;
    Client.Ready += OnReady.Handle;
    Client.ClientErrored += OnClientError.Handle;
    Client.SocketErrored += OnSocketError.Handle;

    //Ticket events
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
    


   
    //Commands
    var commands = Client.UseCommandsNext(new CommandsNextConfiguration {
      StringPrefixes = [
        BotConfig.This.BotPrefix
      ],
      EnableDms = false,
      CaseSensitive = false
    });


    commands.RegisterCommands<ModmailCommands>();
    
  }

  public bool Connected { get; private set; }

  public static ModmailBot This {
    get {
      _instance ??= new ModmailBot(BotConfig.This);
      return _instance;
    }
  }

  public DiscordClient Client { get; }
  // public ServiceProvider Services { get; private set; }

  public static async Task StartAsync() {
    Log.Information("Starting bot");
    var option = await GuildOption.GetAsync();

    _ = This;//init
    var slash = This.Client.UseSlashCommands();

    if (!option.DisableBlacklistSlashCommands) slash.RegisterCommands<BlacklistSlashCommands>();

    if (!option.DisableTicketSlashCommands) slash.RegisterCommands<TicketSlashCommands>();
    await This.Client.ConnectAsync();
    This.Connected = true;

    await Task.Delay(5);

    await This.Client.UpdateStatusAsync(Const.DISCORD_ACTIVITY);
    await DiscordUserInfo.AddOrUpdateAsync(This.Client.CurrentUser);
  }

  public static async Task StopAsync() {
    Log.Information("Stopping bot");
    This.Connected = false;
    await This.Client.DisconnectAsync();
    This.Client.Dispose();
    _instance = null;
  }



  public async Task<DiscordMember?> GetMemberFromAnyGuildAsync(ulong userId) {
    foreach (var guild in Client.Guilds)
      try {
        var member = await guild.Value.GetMemberAsync(userId);
        if (member == null) continue;
        await DiscordUserInfo.AddOrUpdateAsync(member);
        return member;
      }
      catch (Exception ex) {
        Log.Error(ex, "Failed to get member from guild {GuildId} for user {UserId}", guild.Key, userId);
      }

    return null;
  }

  public async Task<DiscordGuild> GetMainGuildAsync() {
    var key = SimpleCacher.CreateKey(nameof(ModmailBot), nameof(GetMainGuildAsync));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(300)) ?? await _get();


    async Task<DiscordGuild> _get() {
      var guildId = BotConfig.This.MainServerId;
      var guild = await Client.GetGuildAsync(guildId);
      if (guild == null) {
        Log.Error("Main guild not found: {GuildId}", guildId);
        throw new NotFoundException(LangKeys.MAIN_GUILD);
      }

      var guildOption = await GuildOption.GetAsync();

      guildOption.Name = guild.Name;
      guildOption.IconUrl = guild.IconUrl;
      guildOption.BannerUrl = guild.BannerUrl;
      await guildOption.UpdateAsync();
      await DiscordUserInfo.AddOrUpdateAsync(guild.Owner);

      return guild;
    }
  }

  public async Task<DiscordChannel> GetLogChannelAsync() {
    var key = SimpleCacher.CreateKey(nameof(ModmailBot), nameof(GetLogChannelAsync));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(60)) ?? await _get();

    async Task<DiscordChannel> _get() {
      var guild = await GetMainGuildAsync();
      var option = await GuildOption.GetAsync();
      if (option is null) throw new ServerIsNotSetupException();

      var logChannel = guild.GetChannel(option.LogChannelId);

      if (logChannel is null) {
        logChannel = await option.ProcessCreateLogChannel(guild);
        Log.Information("Log channel not found, created new log channel {LogChannelId}", logChannel.Id);
      }

      return logChannel;
    }
  }


  public async Task<List<DiscordRole>> GetRoles() {
    var key = SimpleCacher.CreateKey(nameof(ModmailBot), nameof(GetRoles));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(10)) ?? await _get();

    async Task<List<DiscordRole>> _get() {
      var guild = await GetMainGuildAsync();
      var rolesDict = guild.Roles;
      var roles = rolesDict.Values.ToList();
      return roles;
    }
  }
}