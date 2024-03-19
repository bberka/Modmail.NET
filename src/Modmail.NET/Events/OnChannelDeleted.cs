using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Serilog;

namespace Modmail.NET.Events;

public static class OnChannelDeleted
{
  public static async Task Handle(DiscordClient sender, ChannelDeleteEventArgs args) {
    var channel = args.Channel;
    var channelTopic = channel.Topic;
    var guild = channel.Guild;

    var currentUser = await guild.GetMemberAsync(sender.CurrentUser.Id);
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId != Guid.Empty) {
      var dbService = ServiceLocator.Get<IDbService>();
      // var option = await dbService.GetOptionAsync(guild.Id);
      // if (option is null) {
      //   Log.Warning("Option not found for guild: {GuildOptionId}", guild.Id);
      //   return;
      // }
      var ticket = await dbService.GetActiveTicketAsync(ticketId);
      if (ticket is not null) {
        var logChannelId = await dbService.GetLogChannelIdAsync(guild.Id);
        if (logChannelId == 0) {
          Log.Warning("LogChannelId not found in database for guild: {GuildOptionId}", guild.Id);
          return;
        }

        var logChannel = guild.GetChannel(logChannelId);
        if (logChannel is null) {
          Log.Warning("LogChannel not found in guild: {GuildOptionId}", guild.Id);
          return;
        }

        var ticketOpenUser = await guild.GetMemberAsync(ticket.DiscordUserId);
        var logEmbed = ModmailEmbedBuilder.ToLog.TicketClosed(currentUser,
                                                              ticketOpenUser,
                                                              guild,
                                                              ticketId,
                                                              ticket.RegisterDate,
                                                              "Channel was deleted");
        await logChannel.SendMessageAsync(logEmbed);

        var embed = ModmailEmbedBuilder.ToUser.TicketClosed(guild, ticketOpenUser, ticket.GuildOption);
        await ticketOpenUser.SendMessageAsync(embed);

        ticket.ClosedDate = DateTime.Now;
        ticket.IsForcedClosed = true;
        await dbService.UpdateTicketAsync(ticket);
      }
    }
  }
}