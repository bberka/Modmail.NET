using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Aspects;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Entities;

public class GuildOption
{
  [Key]
  public ulong GuildId { get; set; }

  public string Name { get; set; } = "Modmail";
  public string IconUrl { get; set; } = "";

  public string? BannerUrl { get; set; }
  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; } = DateTime.UtcNow;

  public bool IsSensitiveLogging { get; set; } = true;

  public string GreetingMessage { get; set; }
    = "Thank you for reaching out to our team, we'll reply to you as soon as possible. Please help us speed up this process by describing your request in detail.";

  public string ClosingMessage { get; set; } = "Your ticket has been closed. If you have any further questions, feel free to open a new ticket by messaging me again.";

  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }

  public virtual List<GuildTeam> GuildTeams { get; set; }

  public virtual List<Ticket> Tickets { get; set; }
  public virtual List<TicketBlacklist> TicketBlacklists { get; set; }


  [CacheAspect(DoNotCacheIfNull = true, CacheSeconds = 10)]
  public static async Task<GuildOption> GetAsync() {
    var result = await GetNullableAsync();
    if (result is null) {
      throw new ServerIsNotSetupException();
    }

    return result;
  }

  [CacheAspect(DoNotCacheIfNull = true, CacheSeconds = 10)]
  public static async Task<GuildOption?> GetNullableAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildOptions.FirstOrDefaultAsync(x => x.GuildId == BotConfig.This.MainServerId);
  }

  public async Task UpdateAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    this.UpdateDateUtc = DateTime.UtcNow;
    dbContext.GuildOptions.Update(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<ulong> GetLogChannelIdAsync(ulong guildId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildOptions.Where(x => x.GuildId == guildId).Select(x => x.LogChannelId).FirstOrDefaultAsync();
  }

  private async Task AddAsync() {
    await DeleteAllAsync();
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.GuildOptions.Add(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<bool> Any() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildOptions.AnyAsync();
  }

  private static async Task DeleteAllAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var options = await dbContext.GuildOptions.ToListAsync();
    dbContext.GuildOptions.RemoveRange(options);
    await dbContext.SaveChangesAsync();
  }

  public static async Task ProcessSetupAsync(DiscordGuild guild, bool sensitiveLogging, bool takeFeedbackAfterClosing, string? greetingMessage, string? closingMessage) {
    var existingMmOption = await GetNullableAsync();
    if (existingMmOption is not null) {
      throw new MainServerAlreadySetupException();
    }

    var anyServerSetup = await Any();
    if (anyServerSetup) {
      throw new AnotherServerAlreadySetupException();
    }

    var guildId = guild.Id;

    var permissions = await GuildTeamMember.GetPermissionInfoOrHigherAsync(TeamPermissionLevel.Admin);
    var members = await guild.GetAllMembersAsync();
    var roles = guild.Roles;

    var roleListForOverwrites = new List<DiscordRole>();
    var memberListForOverwrites = new List<DiscordMember>();
    foreach (var perm in permissions) {
      var role = roles.FirstOrDefault(x => x.Key == perm.Key && perm.Type == TeamMemberDataType.RoleId);
      if (role.Key != 0) roleListForOverwrites.Add(role.Value);
      var member2 = members.FirstOrDefault(x => x.Id == perm.Key && perm.Type == TeamMemberDataType.UserId);
      if (member2 is not null && member2.Id != 0) memberListForOverwrites.Add(member2);
    }


    var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, memberListForOverwrites, roleListForOverwrites);


    var category = await guild.CreateChannelCategoryAsync(Const.CATEGORY_NAME, permissionOverwrites);
    var logChannel = await guild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category, Texts.MODMAIL_LOG_CHANNEL_TOPIC, permissionOverwrites);
    var categoryId = category.Id;
    var logChannelId = logChannel.Id;
    var guildOption = new GuildOption {
      CategoryId = categoryId,
      GuildId = guild.Id,
      LogChannelId = logChannelId,
      IsSensitiveLogging = sensitiveLogging,
      IsEnabled = true,
      RegisterDateUtc = DateTime.UtcNow,
      TakeFeedbackAfterClosing = takeFeedbackAfterClosing,
      ShowConfirmationWhenClosingTickets = false,
      IconUrl = guild.IconUrl,
      Name = guild.Name,
      BannerUrl = guild.BannerUrl,
    };
    if (!string.IsNullOrEmpty(greetingMessage))
      guildOption.GreetingMessage = greetingMessage;
    if (!string.IsNullOrEmpty(closingMessage))
      guildOption.ClosingMessage = closingMessage;
    await guildOption.AddAsync();

    await logChannel.SendMessageAsync(LogResponses.SetupComplete(guildOption));
  }

  public async Task ProcessConfigureAsync(DiscordGuild guild, bool? sensitiveLogging, bool? takeFeedbackAfterClosing, string? greetingMessage, string? closingMessage) {
    IconUrl = guild.IconUrl;
    Name = guild.Name;
    BannerUrl = guild.BannerUrl;
    UpdateDateUtc = DateTime.UtcNow;
    if (sensitiveLogging.HasValue)
      IsSensitiveLogging = sensitiveLogging.Value;
    if (takeFeedbackAfterClosing.HasValue)
      TakeFeedbackAfterClosing = takeFeedbackAfterClosing.Value;
    if (!string.IsNullOrEmpty(greetingMessage))
      GreetingMessage = greetingMessage;
    if (!string.IsNullOrEmpty(closingMessage))
      ClosingMessage = closingMessage;
    await UpdateAsync();


    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.ConfigurationUpdated(this));
  }
}