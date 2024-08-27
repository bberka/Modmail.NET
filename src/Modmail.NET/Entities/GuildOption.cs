using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Utils;

namespace Modmail.NET.Entities;

public class GuildOption
{
  public ulong GuildId { get; set; }

  [MaxLength(DbLength.NAME)]
  public string Name { get; set; } = "Modmail";

  [MaxLength(DbLength.URL)]
  public string IconUrl { get; set; } = "";

  [MaxLength(DbLength.URL)]
  public string? BannerUrl { get; set; }

  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; } = DateTime.UtcNow;

  public bool IsSensitiveLogging { get; set; } = true;

  [Range(Const.TICKET_TIMEOUT_MIN_ALLOWED_HOURS, Const.TICKET_TIMEOUT_MAX_ALLOWED_HOURS)]
  public long TicketTimeoutHours { get; set; } = Const.DEFAULT_TICKET_TIMEOUT_HOURS;

  [MaxLength(DbLength.BOT_MESSAGE)]
  public string GreetingMessage { get; set; }
    = "Thank you for reaching out to our team, we'll reply to you as soon as possible. Please help us speed up this process by describing your request in detail.";

  [MaxLength(DbLength.BOT_MESSAGE)]
  public string ClosingMessage { get; set; } = "Your ticket has been closed. If you have any further questions, feel free to open a new ticket by messaging me again.";

  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }

  public virtual List<GuildTeam> GuildTeams { get; set; }

  public virtual List<Ticket> Tickets { get; set; }
  public virtual List<TicketBlacklist> TicketBlacklists { get; set; }


  public static async Task<GuildOption> GetAsync() {
    var key = SimpleCacher.CreateKey(nameof(GuildOption), nameof(GetAsync));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(60));

    async Task<GuildOption> _get() {
      var result = await GetNullableAsync();
      if (result is null) {
        throw new ServerIsNotSetupException();
      }

      return result;
    }
  }

  public static async Task<GuildOption?> GetNullableAsync() {
    var key = SimpleCacher.CreateKey(nameof(GuildOption), nameof(GetNullableAsync));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(1));

    async Task<GuildOption?> _get() {
      var dbContext = ServiceLocator.Get<ModmailDbContext>();
      return await dbContext.GuildOptions.FirstOrDefaultAsync(x => x.GuildId == BotConfig.This.MainServerId);
    }
  }

  public async Task UpdateAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    this.UpdateDateUtc = DateTime.UtcNow;
    dbContext.GuildOptions.Update(this);
    var affected = await dbContext.SaveChangesAsync();
    if (affected == 0) {
      throw new Exception("Failed to update guild option");
    }
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

  public static async Task ProcessSetupAsync(DiscordGuild guild,
                                             bool sensitiveLogging,
                                             bool takeFeedbackAfterClosing,
                                             string? greetingMessage,
                                             string? closingMessage,
                                             long? ticketTimeoutHours = null) {
    var existingMmOption = await GetNullableAsync();
    if (existingMmOption is not null) {
      throw new MainServerAlreadySetupException();
    }

    var anyServerSetup = await Any();
    if (anyServerSetup) {
      throw new AnotherServerAlreadySetupException();
    }

    var guildOption = new GuildOption {
      CategoryId = 0,
      LogChannelId = 0,
      GuildId = guild.Id,
      IsSensitiveLogging = sensitiveLogging,
      IsEnabled = true,
      RegisterDateUtc = DateTime.UtcNow,
      TakeFeedbackAfterClosing = takeFeedbackAfterClosing,
      ShowConfirmationWhenClosingTickets = false,
      IconUrl = guild.IconUrl,
      Name = guild.Name,
      BannerUrl = guild.BannerUrl,
      TicketTimeoutHours = Const.DEFAULT_TICKET_TIMEOUT_HOURS
    };
    if (!string.IsNullOrEmpty(greetingMessage))
      guildOption.GreetingMessage = greetingMessage;
    if (!string.IsNullOrEmpty(closingMessage))
      guildOption.ClosingMessage = closingMessage;
    if (ticketTimeoutHours.HasValue) {
      if (ticketTimeoutHours.Value < Const.TICKET_TIMEOUT_MIN_ALLOWED_HOURS || ticketTimeoutHours.Value > Const.TICKET_TIMEOUT_MAX_ALLOWED_HOURS) {
        throw new TicketTimeoutOutOfRangeException();
      }

      guildOption.TicketTimeoutHours = ticketTimeoutHours.Value;
    }

    await guildOption.AddAsync();

    var logChannel = await guildOption.ProcessCreateLogChannel(guild);

    await logChannel.SendMessageAsync(LogResponses.SetupComplete(guildOption));
  }

  public async Task ProcessConfigureAsync(DiscordGuild guild,
                                          bool? sensitiveLogging,
                                          bool? takeFeedbackAfterClosing,
                                          string? greetingMessage,
                                          string? closingMessage,
                                          long? ticketTimeoutHours = null) {
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
    if (ticketTimeoutHours.HasValue) {
      if (ticketTimeoutHours.Value < Const.TICKET_TIMEOUT_MIN_ALLOWED_HOURS || ticketTimeoutHours.Value > Const.TICKET_TIMEOUT_MAX_ALLOWED_HOURS) {
        throw new TicketTimeoutOutOfRangeException();
      }

      TicketTimeoutHours = ticketTimeoutHours.Value;
    }

    await UpdateAsync();


    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.ConfigurationUpdated(this));
  }

  public async Task<DiscordChannel> ProcessCreateLogChannel(DiscordGuild guild) {
    var category = guild.GetChannel(CategoryId);

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

    if (category is null) {
      category = await guild.CreateChannelCategoryAsync(Const.CATEGORY_NAME, permissionOverwrites);
    }

    var logChannel = await guild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category, LangData.This.GetTranslation(LangKeys.MODMAIL_LOG_CHANNEL_TOPIC), permissionOverwrites);
    LogChannelId = logChannel.Id;
    CategoryId = category.Id;
    await UpdateAsync();
    return logChannel;
  }
}