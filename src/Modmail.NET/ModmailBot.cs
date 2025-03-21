using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Commands;
using Modmail.NET.Commands.Slash;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Utils;
using Serilog;
using Serilog.Extensions.Logging;
using NotFoundException = Modmail.NET.Exceptions.NotFoundException;

namespace Modmail.NET;

public class ModmailBot
{
  private readonly IMemoryCache _memoryCache;
  private readonly IServiceProvider _serviceProvider;

  public ModmailBot(IOptions<BotConfig> config,
                    IMemoryCache memoryCache,
                    IServiceProvider serviceProvider) {
    _memoryCache = memoryCache;
    _serviceProvider = serviceProvider;

    if (config.Value.Environment == EnvironmentType.Development)
      Log.Information("Running in development mode");

    Log.Information("Starting Modmail.NET v{Version}", UtilVersion.GetVersion());
    //Define the client
    Client = new DiscordClient(new DiscordConfiguration {
      Token = config.Value.BotToken,
      AutoReconnect = true,
      TokenType = TokenType.Bot,
      Intents = DiscordIntents.All,
      HttpTimeout = TimeSpan.FromSeconds(10),
      LogUnknownEvents = false,
      LoggerFactory = new SerilogLoggerFactory(Log.Logger)
    });

    var eventHandlers = _serviceProvider.GetRequiredService<ModmailEventHandlers>();
    //Define the events
    Client.Heartbeated += eventHandlers.OnHeartbeat;
    Client.Ready += eventHandlers.OnReady;
    Client.ClientErrored += eventHandlers.OnClientError;
    Client.SocketErrored += eventHandlers.OnSocketError;

    //Ticket events
    Client.MessageCreated += eventHandlers.OnMessageCreated;
    Client.ChannelDeleted += eventHandlers.OnChannelDeleted;

    Client.InteractionCreated += eventHandlers.InteractionCreated;
    Client.ComponentInteractionCreated += eventHandlers.ComponentInteractionCreated;
    Client.ModalSubmitted += eventHandlers.ModalSubmitted;

    //FOR USER DATA UPDATE ONLY
    Client.GuildMemberAdded += eventHandlers.OnGuildMemberAdded;
    Client.GuildMemberRemoved += eventHandlers.OnGuildMemberRemoved;
    Client.GuildBanAdded += eventHandlers.OnGuildBanAdded;
    Client.GuildBanRemoved += eventHandlers.OnGuildBanRemoved;
    Client.MessageAcknowledged += eventHandlers.OnMessageAcknowledged;
    Client.UserUpdated += eventHandlers.OnUserUpdated;
    Client.UserSettingsUpdated += eventHandlers.OnUserSettingsUpdated;
    Client.ScheduledGuildEventUserAdded += eventHandlers.OnScheduledGuildEventUserAdded;
    Client.ScheduledGuildEventUserRemoved += eventHandlers.OnScheduledGuildEventUserRemoved;
    Client.MessageReactionAdded += eventHandlers.OnMessageReactionAdded;
    Client.MessageReactionRemoved += eventHandlers.OnMessageReactionRemoved;
    Client.MessageReactionRemovedEmoji += eventHandlers.OnMessageReactionRemovedEmoji;
    Client.MessageReactionsCleared += eventHandlers.OnMessageReactionsCleared;
    Client.MessageDeleted += eventHandlers.OnMessageDeleted;
    Client.MessageUpdated += eventHandlers.OnMessageUpdated;
    Client.ThreadCreated += eventHandlers.OnThreadCreated;


    //Commands
    var commands = Client.UseCommandsNext(new CommandsNextConfiguration {
      StringPrefixes = [
        config.Value.BotPrefix
      ],
      EnableDms = false,
      CaseSensitive = false,
      Services = _serviceProvider
    });


    commands.RegisterCommands<ModmailCommands>();

    var scope = _serviceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var option = sender.Send(new GetGuildOptionQuery(true)).GetAwaiter().GetResult();

    var slash = Client.UseSlashCommands();

    //if there is no option and it is null register commands for first bot run
    //later user can disable commands through option but by default if its not true we must register it.
    if (option?.DisableBlacklistSlashCommands is not true) slash.RegisterCommands<BlacklistSlashCommands>();

    if (option?.DisableTicketSlashCommands is not true) slash.RegisterCommands<TicketSlashCommands>();
  }

  public bool Connected { get; private set; }
  public DiscordClient Client { get; }


  public async Task StartAsync() {
    Log.Information("Starting bot");

    await Client.ConnectAsync();
    Connected = true;
    await Task.Delay(5);

    await Client.UpdateStatusAsync(Const.DISCORD_ACTIVITY);

    var scope = _serviceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    await sender.Send(new UpdateDiscordUserCommand(Client.CurrentUser));
  }

  public async Task StopAsync() {
    Log.Information("Stopping bot");
    Connected = false;
    await Client.DisconnectAsync();
    Client.Dispose();
  }


  public async Task<DiscordMember> GetMemberFromAnyGuildAsync(ulong userId) {
    foreach (var guild in Client.Guilds)
      try {
        var member = await guild.Value.GetMemberAsync(userId);
        if (member == null) continue;
        var sender = _serviceProvider.GetRequiredService<ISender>();
        await sender.Send(new UpdateDiscordUserCommand(member));
        return member;
      }
      catch (Exception ex) {
        Log.Error(ex, "Failed to get member from guild {GuildId} for user {UserId}", guild.Key, userId);
      }

    return null;
  }

  public async Task<DiscordGuild> GetMainGuildAsync() {
    const string cacheKey = "ModmailBot.GetMainGuildAsync";
    return await _memoryCache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300)
    });

    async Task<DiscordGuild> Get(ICacheEntry cacheEntry) {
      var config = _serviceProvider.GetRequiredService<IOptions<BotConfig>>();
      var guildId = config.Value.MainServerId;
      var guild = await Client.GetGuildAsync(guildId);
      if (guild == null) {
        Log.Error("Main guild not found: {GuildId}", guildId);
        throw new NotFoundException(LangKeys.MAIN_GUILD);
      }

      var scope = _serviceProvider.CreateScope();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      var guildOption = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();

      var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
      guildOption.Name = guild.Name;
      guildOption.IconUrl = guild.IconUrl;
      guildOption.BannerUrl = guild.BannerUrl;

      dbContext.Update(guildOption);
      var affected = await dbContext.SaveChangesAsync();
      if (affected == 0) throw new DbInternalException();

      await sender.Send(new UpdateDiscordUserCommand(guild.Owner));
      return guild;
    }
  }

  public async Task<DiscordChannel> GetLogChannelAsync() {
    const string cacheKey = "ModmailBot.GetLogChannelAsync";
    return await _memoryCache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    });

    async Task<DiscordChannel> Get(ICacheEntry cacheEntry) {
      var guild = await GetMainGuildAsync();
      var scope = _serviceProvider.CreateScope();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();

      var option = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();

      var logChannel = guild.GetChannel(option.LogChannelId);

      if (logChannel is null) {
        logChannel = await sender.Send(new ProcessCreateLogChannelCommand(guild));
        Log.Information("Log channel not found, created new log channel {LogChannelId}", logChannel.Id);
      }

      return logChannel;
    }
  }


  public async Task<List<DiscordRole>> GetRoles() {
    const string cacheKey = "ModmailBot.GetRoles";
    return await _memoryCache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
    });

    async Task<List<DiscordRole>> Get(ICacheEntry cacheEntry) {
      var guild = await GetMainGuildAsync();
      var rolesDict = guild.Roles;
      var roles = rolesDict.Values.ToList();
      return roles;
    }
  }
}