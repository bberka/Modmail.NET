using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Events;

public static class OnChannelDeleted
{
  public static async Task Handle(DiscordClient sender, ChannelDeleteEventArgs args) {
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(args.Channel.Topic);
    if (ticketId != Guid.Empty) {
      var auditLogEntry = (await args.Guild.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelDelete)).FirstOrDefault();
      var user = auditLogEntry?.UserResponsible ?? sender.CurrentUser;
      await DiscordUserInfo.AddOrUpdateAsync(user);
      var ticket = await Ticket.GetActiveAsync(ticketId);
      if (ticket is not null) {
        await ticket.CloseTicketAsync(user.Id, Texts.CHANNEL_WAS_DELETED, args.Channel);
      }
    }
  }
}