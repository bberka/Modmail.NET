using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket", "Ticket management commands.")]
[RequireAdmin]
public class TicketSlashCommands : ApplicationCommandModule
{
  [SlashCommand("close", "Close a ticket.")]
  public async Task CloseTicket(InteractionContext ctx,
                                [Option("reason", "Ticket closing reason")]
                                string reason = "") {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;


    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed3 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildOptionId}", ticketId, currentGuild.Id);
      var embed3 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    ticket.ClosedDate = DateTime.Now;
    await dbService.UpdateTicketAsync(ticket);

    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);
    var embed = ModmailEmbedBuilder.ToUser.TicketClosed(ctx.Guild, ticketOpenUser);
    await ticketOpenUser.SendMessageAsync(embed);


    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    var logEmbed = ModmailEmbedBuilder.ToLog.TicketClosed(currentUser, ticketOpenUser, ticketId, ticket.RegisterDate, reason);
    await logChannel.SendMessageAsync(logEmbed);


    var embed2 = ModmailEmbedBuilder.Base("Ticket closed!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);


    await currentChannel.DeleteAsync("ticket_closed");

    Log.Information("Ticket closed: {TicketId} in guild {GuildOptionId}", ticketId, ticket.GuildOption.GuildId);
  }

  [SlashCommand("set-priority", "Set the priority of a ticket.")]
  public async Task SetPriority(InteractionContext ctx,
                                [Option("priority", "Priority of the ticket")]
                                TicketPriority priority) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed4 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildOptionId}", ticketId, currentGuild.Id);

      var embed4 = ModmailEmbedBuilder.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);


      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var oldPriority = ticket.Priority;
    ticket.Priority = priority;
    await dbService.UpdateTicketAsync(ticket);


    // var guildId = ticket.GuildOptionId;
    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);


    var newChName = "";
    switch (priority) {
      case TicketPriority.Normal:
        newChName = Const.NORMAL_PRIORITY_EMOJI + ctx.Channel.Name;
        break;
      case TicketPriority.High:
        newChName = Const.HIGH_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, ticketOpenUser.Username.Trim());
        ;
        break;
      case TicketPriority.Low:
        newChName = Const.LOW_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, ticketOpenUser.Username.Trim());
        ;
        break;
    }

    await ctx.Channel.ModifyAsync(x => { x.Name = newChName; });


    var embed = ModmailEmbedBuilder.ToUser.TicketPriorityChanged(ctx.Guild, ctx.User, oldPriority, priority);
    await ticketOpenUser.SendMessageAsync(embed);

    var embed2 = ModmailEmbedBuilder.ToLog.TicketPriorityChanged(ctx.Guild, ctx.User, oldPriority, priority);
    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed2);


    var embed3 = ModmailEmbedBuilder.Base("Priority set!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed3);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    Log.Information("Priority set: {TicketId} in guild {GuildOptionId}", ticketId, ticket.GuildOption.GuildId);
  }


}