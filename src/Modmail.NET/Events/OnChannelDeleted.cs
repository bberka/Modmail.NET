using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Aspects;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Static;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Events;

public static class OnChannelDeleted
{
  [PerformanceLoggerAspect(ThresholdMs = 3000)]
  public static async Task Handle(DiscordClient sender, ChannelDeleteEventArgs args) {
    const string logMessage = $"[{nameof(OnChannelDeleted)}]{nameof(Handle)}({{ChannelId}})";
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(args.Channel.Topic);
    if (ticketId != Guid.Empty) {
      try {
        var auditLogEntry = (await args.Guild.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelDelete)).FirstOrDefault();
        var user = auditLogEntry?.UserResponsible ?? sender.CurrentUser;
        await DiscordUserInfo.AddOrUpdateAsync(user);
        var ticket = await Ticket.GetActiveTicketAsync(ticketId);
        await ticket.ProcessCloseTicketAsync(user.Id, Texts.CHANNEL_WAS_DELETED, args.Channel);
        Log.Information(logMessage, args.Channel.Id);
      }
      catch (BotExceptionBase ex) {
        Log.Warning(ex, logMessage, args.Channel.Id);
      }
      catch (Exception ex) {
        Log.Error(ex, logMessage, args.Channel.Id);
      }
    }
  }
}