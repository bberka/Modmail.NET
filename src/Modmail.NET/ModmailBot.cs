using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;
using Serilog;
using NotFoundException = Modmail.NET.Exceptions.NotFoundException;

namespace Modmail.NET;

public class ModmailBot
{
  public ModmailBot(DiscordClient client) {
    Client = client;
  }

  public bool Connected => Client.AllShardsConnected;
  public DiscordClient Client { get; }


  public async Task StartAsync() {
    Log.Information("Starting bot");

    await Client.ConnectAsync();

    while (!Client.AllShardsConnected) await Task.Delay(5);

    await Client.UpdateStatusAsync(Const.DiscordActivity);

    var scope = Client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<BotConfig>>();
    await sender.Send(new UpdateDiscordUserCommand(Client.CurrentUser));


    var guildJoined = Client.Guilds.TryGetValue(options.Value.MainServerId, out var guild);
    if (!guildJoined) throw new NotJoinedMainServerException();

    Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} Setting up main server");
    try {
      await sender.Send(new ProcessGuildSetupCommand(Client.CurrentUser.Id, guild));
      Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server setup complete");
    }
    catch (MainServerAlreadySetupException ex) {
      Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server already setup");
    }
    catch (Exception ex) {
      Log.Fatal(ex, $"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server setup exception");
      throw;
    }
  }

  public async Task StopAsync() {
    Log.Information("Stopping bot");
    await Client.DisconnectAsync();
    Client.Dispose();
  }


  public async Task<DiscordMember> GetMemberFromAnyGuildAsync(ulong userId) {
    foreach (var guild in Client.Guilds)
      try {
        var member = await guild.Value.GetMemberAsync(userId);
        if (member == null) continue;
        var scope = Client.ServiceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new UpdateDiscordUserCommand(member));
        return member;
      }
      catch (Exception ex) {
        Log.Error(ex, "Failed to get member from guild {GuildId} for user {UserId}", guild.Key, userId);
      }

    return null;
  }

  public async Task<DiscordGuild> GetMainGuildAsync() {
    var scope = Client.ServiceProvider.CreateScope();
    var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
    const string cacheKey = "ModmailBot.GetMainGuildAsync";
    return await memoryCache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300)
    });

    async Task<DiscordGuild> Get(ICacheEntry cacheEntry) {
      var config = scope.ServiceProvider.GetRequiredService<IOptions<BotConfig>>();
      var guildId = config.Value.MainServerId;
      var guild = await Client.GetGuildAsync(guildId);
      if (guild == null) {
        Log.Error("Main guild not found: {GuildId}", guildId);
        throw new NotFoundException(LangKeys.MAIN_GUILD);
      }

      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      var guildOption = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();

      var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
      guildOption.Name = guild.Name;
      guildOption.IconUrl = guild.IconUrl;
      guildOption.BannerUrl = guild.BannerUrl;

      dbContext.Update(guildOption);
      var affected = await dbContext.SaveChangesAsync();
      if (affected == 0) throw new DbInternalException();

      var user = await guild.GetMemberAsync(guild.OwnerId);
      await sender.Send(new UpdateDiscordUserCommand(user));
      return guild;
    }
  }

  public async Task<DiscordChannel> GetLogChannelAsync() {
    var scope = Client.ServiceProvider.CreateScope();
    var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

    const string cacheKey = "ModmailBot.GetLogChannelAsync";
    return await memoryCache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    });

    async Task<DiscordChannel> Get(ICacheEntry cacheEntry) {
      var guild = await GetMainGuildAsync();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();

      var option = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();

      var logChannel = await guild.GetChannelAsync(option.LogChannelId);

      if (logChannel == null) {
        logChannel = await sender.Send(new ProcessCreateLogChannelCommand(Client.CurrentUser.Id, guild));
        Log.Information("Log channel not found, created new log channel {LogChannelId}", logChannel.Id);
      }

      return logChannel;
    }
  }


  public async Task<List<DiscordRole>> GetRoles() {
    const string cacheKey = "ModmailBot.GetRoles";
    var scope = Client.ServiceProvider.CreateScope();
    var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
    return await memoryCache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
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