using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Events;

public static class OnChannelDeleted
{
  public static async Task Handle(DiscordClient sender, ChannelDeleteEventArgs args) {
    var channel = args.Channel;
    var channelTopic = channel.Topic;
    var guild = channel.Guild;

    var currentUser = sender.CurrentUser;
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId != Guid.Empty) {
      var dbService = ServiceLocator.Get<IDbService>();
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

        ticket.ClosedDateUtc = DateTime.UtcNow;
        ticket.IsForcedClosed = true;
        ticket.CloseReason = Texts.CHANNEL_WAS_DELETED;
        await dbService.UpdateTicketAsync(ticket);


        var privateChannel = (DiscordDmChannel)await ModmailBot.This.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
        var user = privateChannel.Recipients.FirstOrDefault(x => x.Id == ticket.DiscordUserInfoId);
        var ticketOpenUser = await ModmailBot.This.GetMemberFromAnyGuildAsync(ticket.DiscordUserInfoId);
        if (privateChannel is null || user is null) {
          Log.Warning("TicketOpenUser not found for ticket: {TicketId}", ticketId);
          return;
        }

        var logEmbed = ModmailEmbeds.ToLog.TicketClosed(currentUser,
                                                        user,
                                                        guild,
                                                        ticketId,
                                                        ticket.RegisterDateUtc,
                                                        Texts.CHANNEL_WAS_DELETED);
        await logChannel.SendMessageAsync(logEmbed);

        var embed = ModmailEmbeds.ToUser.TicketClosed(guild, ticket.GuildOption);
        await privateChannel.SendMessageAsync(embed);

        if (ticket.GuildOption.TakeFeedbackAfterClosing) {
          var interactionFeedback = ModmailInteractions.CreateFeedbackInteraction(ticketId, guild);
          await privateChannel.SendMessageAsync(interactionFeedback);
        }
      }
    }
  }
}