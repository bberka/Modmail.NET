using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket-type", "Commands for managing ticket types")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
[UpdateUserInformation]
[RequireMainServer]
public class TicketTypeSlashCommands : ApplicationCommandModule
{
  [SlashCommand("create", "Create a new ticket type")]
  public async Task CreateTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")]
                                     string name,
                                     [Option("embed-message-title", "The title of the embed message")]
                                     string embedMessageTitle,
                                     [Option("embed-message-content", "The content of the embed message")]
                                     string embedMessageContent,
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

    var id = Guid.NewGuid();
    var idClean = id.ToString().Replace("-", "");


    if (order > int.MaxValue || order < int.MinValue) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.INVALID_ORDER, Texts.INVALID_ORDER_DESCRIPTION));
      return;
    }

    var exists = await TicketType.ExistsAsync(name);
    if (exists) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.TICKET_TYPE_EXISTS, string.Format(Texts.TICKET_TYPE_EXISTS_DESCRIPTION, name)));
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
      EmbedMessageTitle = embedMessageTitle,
      EmbedMessageContent = embedMessageContent,
    };
    await ticketType.AddAsync();


    await ctx.EditResponseAsync(Webhooks.Success(Texts.TICKET_TYPE_CREATED, string.Format(Texts.TICKET_TYPE_CREATED_DESCRIPTION, name)));
  }

  [SlashCommand("update", "Update existing ticket type")]
  public async Task UpdateTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")] [Autocomplete(typeof(TicketTypeProvider))]
                                     string name,
                                     [Option("embed-message-title", "The title of the embed message")]
                                     string embedMessageTitle,
                                     [Option("embed-message-content", "The content of the embed message")]
                                     string embedMessageContent,
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

    var id = Guid.NewGuid();
    var idClean = id.ToString().Replace("-", "");


    if (order > int.MaxValue || order < int.MinValue) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.INVALID_ORDER, Texts.INVALID_ORDER_DESCRIPTION));
      return;
    }

    var ticketType = await TicketType.GetAsync(name);
    if (ticketType is null) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.TICKET_TYPE_EXISTS, string.Format(Texts.TICKET_TYPE_EXISTS_DESCRIPTION, name)));
      return;
    }

    if (emoji != null)
      ticketType.Emoji = emoji;
    if (description != null)
      ticketType.Description = description;
    if (order != 0)
      ticketType.Order = (int)order;
    if (string.IsNullOrEmpty(embedMessageTitle))
      ticketType.EmbedMessageTitle = embedMessageTitle;
    if (string.IsNullOrEmpty(embedMessageContent))
      ticketType.EmbedMessageContent = embedMessageContent;

    await ticketType.UpdateAsync();

    await ctx.EditResponseAsync(Webhooks.Success(Texts.TICKET_TYPE_UPDATED, string.Format(Texts.TICKET_TYPE_UPDATED_DESCRIPTION, name)));
  }

  [SlashCommand("delete", "Delete a ticket type")]
  public async Task DeleteTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")]
                                     string name
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var ticketType = await TicketType.GetAsync(name);
    if (ticketType is null) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.TICKET_TYPE_NOT_FOUND, string.Format(Texts.TICKET_TYPE_NOT_FOUND_DESCRIPTION, name)));
      return;
    }

    await ticketType.RemoveAsync();
    await ctx.EditResponseAsync(Webhooks.Error(Texts.TICKET_TYPE_DELETED, string.Format(Texts.TICKET_TYPE_DELETED_DESCRIPTION, name)));
  }

  [SlashCommand("list", "List all ticket types")]
  public async Task ListTicketTypes(InteractionContext ctx) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var ticketTypes = await TicketType.GetAllAsync();
    if (ticketTypes.Count == 0) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.NO_TICKET_TYPES_FOUND));
      return;
    }

    await ctx.EditResponseAsync(Webhooks.Info(Texts.TICKET_TYPES,
                                              string.Join(Environment.NewLine, ticketTypes.Select(x => $"`{x.Name}` - {x.Description}"))));
  }

  [SlashCommand("get", "Gets the ticket type for the current ticket channel")]
  public async Task GetTicketType(InteractionContext ctx) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var ticketType = await TicketType.GetByChannelIdAsync(ctx.Channel.Id);
    if (ticketType is null) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.TICKET_TYPE_NOT_FOUND, Texts.TICKET_TYPE_NOT_FOUND_DESCRIPTION));
      return;
    }

    await ctx.EditResponseAsync(Webhooks.Info(Texts.TICKET_TYPE, $"`{ticketType.Name}` - {ticketType.Description}"));
  }
}