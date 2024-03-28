using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

[Index(nameof(DiscordUserId), nameof(GuildId), IsUnique = true)]
public class TicketBlacklist
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public string? Reason { get; set; }

  [ForeignKey(nameof(DiscordUserInfo))]
  public ulong DiscordUserId { get; set; }

  [ForeignKey(nameof(GuildOption))]
  public ulong GuildId { get; set; }

  //FK
  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  public virtual GuildOption GuildOption { get; set; }

  public static async Task<bool> IsBlacklistedAsync(ulong userId) {
    if (userId == 0) throw new InvalidUserIdException();
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketBlacklists.AnyAsync(x => x.DiscordUserId == userId);
  }


  public static async Task<List<TicketBlacklist>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketBlacklists.ToListAsync();
    if (result.Count == 0) {
      throw new NoBlacklistedUsersFoundException();
    }

    return result;
  }

  public static async Task<TicketBlacklist> GetAsync(ulong userId) {
    if (userId == 0) {
      throw new InvalidUserIdException();
    }

    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketBlacklists.FirstOrDefaultAsync(x => x.DiscordUserId == userId);
    if (result is null) {
      throw new UserIsNotBlacklistedException();
    }

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
    var activeTicket = await Ticket.GetActiveTicketAsync(userId);
    await activeTicket.ProcessCloseTicketAsync(userId, Texts.TICKET_CLOSED_DUE_TO_BLACKLIST, dontSendFeedbackMessage: true);

    var activeBlock = await TicketBlacklist.IsBlacklistedAsync(userId);
    if (activeBlock) {
      throw new UserAlreadyBlacklistedException();
    }


    var blackList = new TicketBlacklist() {
      Id = Guid.NewGuid(),
      Reason = reason,
      GuildId = BotConfig.This.MainServerId,
      DiscordUserId = userId,
      RegisterDateUtc = DateTime.UtcNow,
    };
    await blackList.AddAsync();

    var user = await Entities.DiscordUserInfo.GetAsync(userId);
    var modUser = await Entities.DiscordUserInfo.GetAsync(modId);

    var embedLog = EmbedLog.BlacklistAdded(modUser, user, reason);
    await logChannel.SendMessageAsync(embedLog);


    if (notifyUser) {
      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
      if (member is not null) {
        var dmEmbed = EmbedUser.YouHaveBeenBlacklisted(reason);
        await member.SendMessageAsync(dmEmbed);
      }
    }
  }

  public async Task ProcessRemoveUserFromBlacklist(ulong authorUserId, ulong userId, bool notifyUser) {
    await this.RemoveAsync();
    var modUser = await DiscordUserInfo.GetAsync(authorUserId);
    var userInfo = await DiscordUserInfo.GetAsync(userId);
    var embedLog = EmbedLog.BlacklistRemoved(modUser, userInfo);
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(embedLog);
    if (notifyUser) {
      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(userId);
      if (member is not null) {
        var dmEmbed = EmbedUser.YouHaveBeenRemovedFromBlacklist(modUser);
        await member.SendMessageAsync(dmEmbed);
      }
    }
  }
}