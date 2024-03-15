using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Common;
using Modmail.NET.Database;

namespace Modmail.NET.Events;

public static class ChannelEventHandlers
{
  public static async Task OnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs args) {
    var channel = args.Channel;
    var channelTopic = channel.Topic;
    var guild = channel.Guild;
    var currentUser = await guild.GetMemberAsync(sender.CurrentUser.Id);
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId != Guid.Empty) {
      await using var db = new ModmailDbContext();
      var ticket = await db.GetActiveModmailAsync(ticketId);
      if (ticket is not null) {
        
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        var ticketOpenUser = await guild.GetMemberAsync(ticket.DiscordUserId);
        var logEmbed = ModmailEmbedBuilder.ToLog.TicketClosed(currentUser,ticketOpenUser, ticketId,ticket.RegisterDate,"Channel was deleted");
        await logChannel.SendMessageAsync(embed: logEmbed);

        var embed = ModmailEmbedBuilder.ToUser.TicketClosed(guild,ticketOpenUser);
        await ticketOpenUser.SendMessageAsync(embed);
    
        ticket.ClosedDate = DateTime.Now;
        ticket.IsForcedClosed = true;
        await db.SaveChangesAsync();
      }
    }
  }
}