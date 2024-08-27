using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Entities;

public class TicketBlacklist
{
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  [MaxLength(DbLength.REASON)]
  public string? Reason { get; set; }

  public ulong DiscordUserId { get; set; }
  
  public DiscordUserInfo DiscordUser { get; set; }


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

  public static async Task ProcessAddUserToBlacklist(ulong modId, ulong userId, string reason, bool notifyUser) {
    // var option = await GuildOption.GetAsync();

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    var activeTicket = await Ticket.GetActiveTicketNullableAsync(userId);
    if (activeTicket is not null) {
      await activeTicket.ProcessCloseTicketAsync(userId, LangData.This.GetTranslation(LangKeys.TICKET_CLOSED_DUE_TO_BLACKLIST), dontSendFeedbackMessage: true);
    }

    var activeBlock = await IsBlacklistedAsync(userId);
    if (activeBlock) throw new UserAlreadyBlacklistedException();


    var blackList = new TicketBlacklist() {
      Id = Guid.NewGuid(),
      Reason = reason,
      DiscordUserId = userId,
      RegisterDateUtc = DateTime.UtcNow
    };
    await blackList.AddAsync();

    var user = await DiscordUserInfo.GetAsync(userId);
    var modUser = await DiscordUserInfo.GetAsync(modId);

    var embedLog = LogResponses.BlacklistAdded(modUser, user, reason);
    await logChannel.SendMessageAsync(embedLog);


    if (notifyUser) {
      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenBlacklisted(reason);
        await member.SendMessageAsync(dmEmbed);
      }
    }
  }

  public async Task ProcessRemoveUserFromBlacklist(ulong authorUserId, ulong userId, bool notifyUser) {
    await RemoveAsync();
    var modUser = await DiscordUserInfo.GetAsync(authorUserId);
    var userInfo = await DiscordUserInfo.GetAsync(userId);
    var embedLog = LogResponses.BlacklistRemoved(modUser, userInfo);
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(embedLog);
    if (notifyUser) {
      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(userId);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenRemovedFromBlacklist(modUser);
        await member.SendMessageAsync(dmEmbed);
      }
    }
  }
}