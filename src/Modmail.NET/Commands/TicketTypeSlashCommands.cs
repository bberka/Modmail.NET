using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket-type", "Commands for managing ticket types")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
public class TicketTypeSlashCommands : ApplicationCommandModule
{
  [SlashCommand("create", "Create a new ticket type")]
  public async Task CreateTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")]
                                     string name,
                                     [Option("emoji", "The emoji used for this ticket type")]
                                     DiscordEmoji? emoji = null,
                                     // [Option("color-hex-code", "The color hex code used for this ticket type")]
                                     // string? colorHexCode = null,
                                     [Option("description", "The description of the ticket type")]
                                     string? description = null,
                                     [Option("order", "The order of the ticket type")]
                                     long order = 0
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    if (ctx.Guild.Id != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var id = Guid.NewGuid();
    var idClean = id.ToString().Replace("-", "");
    var dbService = ServiceLocator.Get<IDbService>();

    if (order > int.MaxValue || order < int.MinValue) {
      var embed1 = ModmailEmbeds.Base(Texts.INVALID_ORDER, Texts.INVALID_ORDER_DESCRIPTION, DiscordColor.Red);
      var webHook1 = new DiscordWebhookBuilder()
        .AddEmbed(embed1);
      await ctx.EditResponseAsync(webHook1);
      return;
    }

    var exists = await dbService.TicketTypeExists(name);
    if (exists) {
      var embed1 = ModmailEmbeds.Base(Texts.TICKET_TYPE_EXISTS, string.Format(Texts.TICKET_TYPE_EXISTS_DESCRIPTION, name), DiscordColor.Red);
      var webHook1 = new DiscordWebhookBuilder()
        .AddEmbed(embed1);
      await ctx.EditResponseAsync(webHook1);
      return;
    }


    var ticketType = new TicketType {
      Id = id,
      Key = idClean,
      Name = name,
      Emoji = emoji,
      Description = description,
      Order = (int)order,
      RegisterDateUtc = DateTime.UtcNow,
    };
    await dbService.AddTicketTypeAsync(ticketType);

    var embed2 = ModmailEmbeds.Base(Texts.TICKET_TYPE_CREATED, string.Format(Texts.TICKET_TYPE_CREATED_DESCRIPTION, name), DiscordColor.Green);
    var webHook2 = new DiscordWebhookBuilder()
      .AddEmbed(embed2);
    await ctx.EditResponseAsync(webHook2);
  }

  [SlashCommand("delete", "Delete a ticket type")]
  public async Task DeleteTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")]
                                     string name
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    if (ctx.Guild.Id != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var ticketType = await dbService.GetTicketTypeByNameAsync(name);
    if (ticketType is null) {
      var embed1 = ModmailEmbeds.Base(Texts.TICKET_TYPE_NOT_FOUND, string.Format(Texts.TICKET_TYPE_NOT_FOUND_DESCRIPTION, name), DiscordColor.Red);
      var webHook1 = new DiscordWebhookBuilder()
        .AddEmbed(embed1);
      await ctx.EditResponseAsync(webHook1);
      return;
    }

    await dbService.RemoveTicketTypeAsync(ticketType);
    var embed2 = ModmailEmbeds.Base(Texts.TICKET_TYPE_DELETED, string.Format(Texts.TICKET_TYPE_DELETED_DESCRIPTION, name), DiscordColor.Green);
    var webHook2 = new DiscordWebhookBuilder()
      .AddEmbed(embed2);
    await ctx.EditResponseAsync(webHook2);
  }

  [SlashCommand("list", "List all ticket types")]
  public async Task ListTicketTypes(InteractionContext ctx) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    if (ctx.Guild.Id != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var ticketTypes = await dbService.GetEnabledTicketTypesAsync();
    var embed = ModmailEmbeds.Base(Texts.TICKET_TYPES,
                                   string.Join(Environment.NewLine, ticketTypes.Select(x => $"`{x.Name}` - {x.Description}")),
                                   DiscordColor.Blurple);
    var webHook = new DiscordWebhookBuilder()
      .AddEmbed(embed);
    await ctx.EditResponseAsync(webHook);
  }

  [SlashCommand("get", "Gets the ticket type for the current ticket channel")]
  public async Task GetTicketType(InteractionContext ctx) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    if (ctx.Guild.Id != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();
    var ticketType = await dbService.GetTicketTypeByChannelIdAsync(ctx.Channel.Id);
    if (ticketType is null) {
      var embed1 = ModmailEmbeds.Base(Texts.TICKET_TYPE_NOT_FOUND, Texts.TICKET_TYPE_NOT_FOUND_DESCRIPTION, DiscordColor.Red);
      var webHook1 = new DiscordWebhookBuilder()
        .AddEmbed(embed1);
      await ctx.EditResponseAsync(webHook1);
      return;
    }

    var embed = ModmailEmbeds.Base(Texts.TICKET_TYPE,
                                   $"`{ticketType.Name}` - {ticketType.Description}",
                                   DiscordColor.Blurple);
    var webHook = new DiscordWebhookBuilder()
      .AddEmbed(embed);
    await ctx.EditResponseAsync(webHook);
  }

  [SlashCommand("set", "Sets the ticket type for the current ticket channel")]
  public async Task SetTicketType(InteractionContext ctx,
                                  [Autocomplete(typeof(TicketTypeProvider))] [Option("name", "The name of the ticket type")]
                                  string name,
                                  [Option("overwrite", "Whether to overwrite the existing ticket type")]
                                  bool overwrite) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    if (ctx.Guild.Id != MMConfig.This.MainServerId) {
      var embed4 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var guildOption = await dbService.GetOptionAsync(ctx.Guild.Id);
    if (guildOption is null) {
      var embed2 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, "", DiscordColor.Green);
      var webHook2 = new DiscordWebhookBuilder()
        .AddEmbed(embed2);
      await ctx.EditResponseAsync(webHook2);
      return;
    }

    var ticketType = await dbService.GetTicketTypeByNameAsync(name);
    if (ticketType is null) {
      var embed1 = ModmailEmbeds.Base(Texts.TICKET_TYPE_NOT_FOUND, string.Format(Texts.TICKET_TYPE_NOT_FOUND_DESCRIPTION, name), DiscordColor.Red);
      var webHook1 = new DiscordWebhookBuilder()
        .AddEmbed(embed1);
      await ctx.EditResponseAsync(webHook1);
      return;
    }

    var channelTopicStr = ctx.Channel.Topic;
    var parsedTicketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopicStr);
    if (parsedTicketId == null || parsedTicketId == Guid.Empty) {
      var embed2 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL, "", DiscordColor.Red);
      var webHook2 = new DiscordWebhookBuilder()
        .AddEmbed(embed2);
      await ctx.EditResponseAsync(webHook2);
      return;
    }

    var ticket = await dbService.GetActiveTicketAsync(parsedTicketId);
    if (ticket is null) {
      var embed2 = ModmailEmbeds.Base(Texts.TICKET_NOT_FOUND, "", DiscordColor.Red);
      var webHook2 = new DiscordWebhookBuilder()
        .AddEmbed(embed2);
      await ctx.EditResponseAsync(webHook2);
      return;
    }

    ticket.TicketTypeId = ticketType.Id;
    await dbService.UpdateTicketAsync(ticket);


    var embedTypeSelectedToMailCh = ModmailEmbeds.ToMail.TicketTypeChanged(ctx.User, ticketType);
    await ctx.Channel.SendMessageAsync(embedTypeSelectedToMailCh);


    var embedTypeSelectedToLogCh = ModmailEmbeds.ToLog.TicketTypeSelected(ctx.User, ticketType, ticket);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(guildOption.LogChannelId);
    await logChannel.SendMessageAsync(embedTypeSelectedToLogCh);

    var wh = new DiscordWebhookBuilder()
      .AddEmbed(ModmailEmbeds.Base(Texts.TICKET_TYPE_CHANGED, "", ModmailEmbeds.TicketTypeChangedColor));
    // .AddEmbed(ModmailEmbeds.Base(Texts.TICKET_TYPE_CHANGED, string.Format(Texts.TICKET_TYPE_CHANGED_MESSAGE_TO_MAIL, ticketType.Name, ticketType.Description), DiscordColor.Green);
    await ctx.EditResponseAsync(wh);
  }
}