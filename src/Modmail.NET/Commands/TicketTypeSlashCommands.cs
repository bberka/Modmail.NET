using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket-type", "Commands for managing ticket types")]
public class TicketTypeSlashCommands : ApplicationCommandModule
{
  [SlashCommand("create", "Create a new ticket type")]
  public async Task CreateTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")]
                                     string name,
                                     [Option("emoji", "The emoji used for this ticket type")]
                                     DiscordEmoji? emoji = null,
                                     [Option("color-hex-code", "The color hex code used for this ticket type")]
                                     string? colorHexCode = null,
                                     [Option("description", "The description of the ticket type")]
                                     string? description = null,
                                     [Option("order", "The order of the ticket type")]
                                     long order = 0,
                                     [Option("enabled", "Whether the ticket type is enabled")]
                                     bool isEnabled = true
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
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
      // ColorHexCode = colorHexCode,
      Description = description,
      Order = (int)order,
      RegisterDateUtc = DateTime.UtcNow,
      IsEnabled = isEnabled,
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
    var dbService = ServiceLocator.Get<IDbService>();

    var ticketTypes = await dbService.GetEnabledTicketTypesAsync();
    var embed = ModmailEmbeds.Base(Texts.TICKET_TYPES,
                                   string.Join(Environment.NewLine, ticketTypes.Select(x => $"`{x.Name}` - {x.Description}")),
                                   DiscordColor.Blurple);
    var webHook = new DiscordWebhookBuilder()
      .AddEmbed(embed);
    await ctx.EditResponseAsync(webHook);
  }
}