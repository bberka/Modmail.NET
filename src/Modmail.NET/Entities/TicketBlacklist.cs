using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Entities;

public sealed class TicketBlacklist
{
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  [MaxLength(DbLength.REASON)]
  public string? Reason { get; set; }

  public ulong DiscordUserId { get; set; }

  public DiscordUserInfo? DiscordUser { get; set; }


  public static async Task<bool> IsBlacklistedAsync(ulong userId) {
    if (userId == 0) throw new InvalidUserIdException();
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketBlacklists.AnyAsync(x => x.DiscordUserId == userId);
  }


  public static async Task<List<TicketBlacklist>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketBlacklists.ToListAsync();
    if (result.Count == 0) throw new EmptyListResultException(LangKeys.BLACKLISTED_USERS);

    return result;
  }

  public static async Task<TicketBlacklist> GetAsync(ulong userId) {
    if (userId == 0) throw new InvalidUserIdException();

    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketBlacklists.FirstOrDefaultAsync(x => x.DiscordUserId == userId);
    if (result is null) throw new UserIsNotBlacklistedException();

    return result;
  }

  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketBlacklists.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task RemoveAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketBlacklists.Remove(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task ProcessAddUserToBlacklist(ulong userId, string? reason = null, ulong modId = 0) {
    // var option = await GuildOption.GetAsync();

    if (modId == 0) modId = ModmailBot.This.Client.CurrentUser.Id; //TODO: Get author from web or set owner user id

    var activeTicket = await Ticket.GetActiveTicketNullableAsync(userId);
    if (activeTicket is not null) await activeTicket.ProcessCloseTicketAsync(userId, LangData.This.GetTranslation(LangKeys.TICKET_CLOSED_DUE_TO_BLACKLIST), dontSendFeedbackMessage: true);

    var activeBlock = await IsBlacklistedAsync(userId);
    if (activeBlock) throw new UserAlreadyBlacklistedException();

    if (string.IsNullOrEmpty(reason)) reason = LangData.This.GetTranslation(LangKeys.NO_REASON_PROVIDED);

    var blackList = new TicketBlacklist {
      Id = Guid.NewGuid(),
      Reason = reason,
      DiscordUserId = userId,
      RegisterDateUtc = DateTime.UtcNow
    };
    await blackList.AddAsync();

    _ = Task.Run(async () => {
      //Don't await this task
      var user = await DiscordUserInfo.GetAsync(userId);
      var modUser = await DiscordUserInfo.GetAsync(modId);

      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var embedLog = LogResponses.BlacklistAdded(modUser, user, reason);
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(embedLog);
      }

      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenBlacklisted(reason);
        await member.SendMessageAsync(dmEmbed);
      }
    });
  }

  public async Task ProcessRemoveUserFromBlacklist(ulong userId, ulong authorUserId = 0) {
    await RemoveAsync();
    if (authorUserId == 0) authorUserId = ModmailBot.This.Client.CurrentUser.Id; //TODO: Get author from web or set owner user id

    _ = Task.Run(async () => {
      //Don't await this task
      var modUser = await DiscordUserInfo.GetAsync(authorUserId);
      var userInfo = await DiscordUserInfo.GetAsync(userId);
      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var embedLog = LogResponses.BlacklistRemoved(modUser, userInfo);
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(embedLog);
      }

      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(userId);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenRemovedFromBlacklist(modUser);
        await member.SendMessageAsync(dmEmbed);
      }
    });
  }
}