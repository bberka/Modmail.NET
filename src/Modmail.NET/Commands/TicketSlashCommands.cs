using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket", "Ticket management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
public class TicketSlashCommands : ApplicationCommandModule
{
  [SlashCommand("close", "Close a ticket.")]
  public async Task CloseTicket(InteractionContext ctx,
                                [Option("reason", "Ticket closing reason")]
                                string? reason = null) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;


    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildOptionId}", ticketId, currentGuild.Id);
      var embed3 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var guildOption = ticket.GuildOption;
    ticket.ClosedDateUtc = DateTime.UtcNow;
    ticket.CloseReason = reason;
    await dbService.UpdateTicketAsync(ticket);

    await currentChannel.DeleteAsync("ticket_closed");

    var privateChannel = (DiscordDmChannel)await ModmailBot.This.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
    var user = privateChannel.Recipients.FirstOrDefault(x => x.Id == ticket.DiscordUserInfoId);
    if (privateChannel is null || user is null) {
      Log.Warning("TicketOpenUser not found for ticket: {TicketId}", ticketId);
      return;
    }

    var embed = ModmailEmbeds.ToUser.TicketClosed(ctx.Guild, guildOption);
    await privateChannel.SendMessageAsync(embed);

    if (guildOption.TakeFeedbackAfterClosing) {
      var interactionFeedback = ModmailInteractions.CreateFeedbackInteraction(ticketId, currentGuild);
      await privateChannel.SendMessageAsync(interactionFeedback);
    }

    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    var logEmbed = ModmailEmbeds.ToLog.TicketClosed(currentUser,
                                                    user,
                                                    ticketId,
                                                    ticket.RegisterDateUtc,
                                                    reason);
    await logChannel.SendMessageAsync(logEmbed);


    var embed2 = ModmailEmbeds.Base(Texts.TICKET_CLOSED, "", DiscordColor.Green);
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

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildOptionId}", ticketId, currentGuild.Id);

      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);


      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var oldPriority = ticket.Priority;
    ticket.Priority = priority;
    await dbService.UpdateTicketAsync(ticket);


    // var guildId = ticket.GuildOptionId;
    var privateChannel = (DiscordDmChannel)await ModmailBot.This.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
    var ticketOpenUser = privateChannel.Recipients.FirstOrDefault(x => x.Id == ticket.DiscordUserInfoId);
    if (privateChannel is null || ticketOpenUser is null) {
      Log.Warning("TicketOpenUser not found for ticket: {TicketId}", ticketId);
      return;
    }

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


    var embed = ModmailEmbeds.ToUser.TicketPriorityChanged(ctx.Guild, ctx.User, oldPriority, priority, ticket.Anonymous);
    await privateChannel.SendMessageAsync(embed);

    var embed2 = ModmailEmbeds.ToLog.TicketPriorityChanged(ctx.User, ticket, oldPriority, priority, ticket.Anonymous);
    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed2);

    var embedMailTicketPriorityChanged = ModmailEmbeds.ToMail.TicketPriorityChanged(ctx.User, oldPriority, priority);
    var mailChannel = currentGuild.GetChannel(ticket.ModMessageChannelId);
    await mailChannel.SendMessageAsync(embedMailTicketPriorityChanged);

    var embed3 = ModmailEmbeds.Base("Priority set!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed3);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    Log.Information("Priority set: {TicketId} in guild {GuildOptionId}", ticketId, ticket.GuildOption.GuildId);
  }


  [SlashCommand("add-note", "Add a note to a ticket.")]
  public async Task AddNote(InteractionContext ctx,
                            [Option("note", "Note to add")] string note) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildOptionId}", ticketId, currentGuild.Id);

      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var noteEntity = new TicketNote {
      TicketId = ticketId,
      Content = note,
      DiscordUserInfoId = currentUser.Id,
      Username = currentUser.GetUsername()
    };
    await dbService.AddNoteAsync(noteEntity);

    var embed = ModmailEmbeds.ToLog.NoteAdded(ctx.User, note, ticket);
    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed);

    var embed2 = ModmailEmbeds.Base("Note added!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    var mailChannel = currentGuild.GetChannel(ticket.ModMessageChannelId);

    var embed3 = ModmailEmbeds.ToMail.NoteAdded(ctx.User, note);
    await mailChannel.SendMessageAsync(embed3);

    Log.Information("Note added: {TicketId} in guild {GuildOptionId}", ticketId, ticket.GuildOption.GuildId);
  }

  [SlashCommand("toggle-anonymous", "Toggle anonymous mode for a ticket.")]
  public async Task ToggleAnonymous(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var ticket = await dbService.GetActiveTicketAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildOptionId}", ticketId, currentGuild.Id);

      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    ticket.Anonymous = !ticket.Anonymous;
    await dbService.UpdateTicketAsync(ticket);

    var embed = ModmailEmbeds.ToLog.AnonymousToggled(ctx.User, ticket, ticket.Anonymous);
    var logChannelId = await dbService.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed);

    var embed2 = ModmailEmbeds.Base("Anonymous mode toggled!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    var mailChannel = currentGuild.GetChannel(ticket.ModMessageChannelId);

    var embed3 = ModmailEmbeds.ToMail.AnonymousToggled(ctx.User, ticket.Anonymous);
    await mailChannel.SendMessageAsync(embed3);

    Log.Information("Anonymous mode toggled: {TicketId} in guild {GuildOptionId}", ticketId, ticket.GuildOption.GuildId);
  }
}