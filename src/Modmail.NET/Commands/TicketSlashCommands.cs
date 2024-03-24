using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket", "Ticket management commands.")]
[UpdateUserInformation]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
[RequireMainServer]
[RequireTicketChannel]
public class TicketSlashCommands : ApplicationCommandModule
{
  [SlashCommand("close", "Close a ticket.")]
  public async Task CloseTicket(InteractionContext ctx,
                                [Option("reason", "Ticket closing reason")]
                                string? reason = null) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    // if (ctx.Guild.Id != MMConfig.This.MainServerId) {
    //   await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Error(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER));
    //   return;
    // }

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    // if (ticketId == Guid.Empty) {
    //   await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Error(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL));
    //   return;
    // }


    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      //The reason why this is before because channel will be deleted after calling CloseTicketAsync
      await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Success(Texts.TICKET_CLOSED));
      await ticket.CloseTicketAsync(ctx.User.Id, reason, ctx.Channel);
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Success(Texts.TICKET_NOT_FOUND));
    }
  }

  [SlashCommand("set-priority", "Set the priority of a ticket.")]
  public async Task SetPriority(InteractionContext ctx,
                                [Option("priority", "Priority of the ticket")]
                                TicketPriority priority) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    // if (ctx.Guild.Id != MMConfig.This.MainServerId) {
    //   await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Error(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER));
    //   return;
    // }

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    // if (ticketId == Guid.Empty) {
    //   await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Error(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL));
    //   return;
    // }


    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      await ticket.ChangePriority(ctx.User.Id, priority, ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Success(Texts.TICKET_PRIORITY_CHANGED));
      return;
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Success(Texts.TICKET_NOT_FOUND));
    }
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

    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildId}", ticketId, currentGuild.Id);

      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var noteEntity = new TicketNote {
      TicketId = ticketId,
      Content = note,
      DiscordUserInfoId = currentUser.Id,
      Username = currentUser.GetUsername(),
      RegisterDateUtc = DateTime.UtcNow,
    };
    await noteEntity.AddAsync();

    var embed = ModmailEmbeds.ToLog.NoteAdded(ctx.User, note, ticket);
    var logChannelId = await GuildOption.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed);

    var embed2 = ModmailEmbeds.Base("Note added!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    var mailChannel = currentGuild.GetChannel(ticket.ModMessageChannelId);

    var embed3 = ModmailEmbeds.ToMail.NoteAdded(ctx.User, note);
    await mailChannel.SendMessageAsync(embed3);

    Log.Information("Note added: {TicketId} in guild {GuildId}", ticketId, ticket.GuildOption.GuildId);
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

    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is null) {
      Log.Verbose("Active ticket not found: {TicketId} {GuildId}", ticketId, currentGuild.Id);

      var embed4 = ModmailEmbeds.Base("This command can only be used in a ticket channel.", "", DiscordColor.Green);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    ticket.Anonymous = !ticket.Anonymous;
    await ticket.UpdateAsync();

    var embed = ModmailEmbeds.ToLog.AnonymousToggled(ctx.User, ticket, ticket.Anonymous);
    var logChannelId = await GuildOption.GetLogChannelIdAsync(ticket.GuildOption.GuildId);
    var logChannel = currentGuild.GetChannel(logChannelId);
    await logChannel.SendMessageAsync(embed);

    var embed2 = ModmailEmbeds.Base("Anonymous mode toggled!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);

    var mailChannel = currentGuild.GetChannel(ticket.ModMessageChannelId);

    var embed3 = ModmailEmbeds.ToMail.AnonymousToggled(ctx.User, ticket.Anonymous);
    await mailChannel.SendMessageAsync(embed3);

    Log.Information("Anonymous mode toggled: {TicketId} in guild {GuildId}", ticketId, ticket.GuildOption.GuildId);
  }
}