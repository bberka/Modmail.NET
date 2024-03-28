using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Providers;
using Modmail.NET.Static;
using Serilog;

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
                                     DiscordEmoji emoji,
                                     [Option("description", "The description of the ticket type")]
                                     string? description = null,
                                     [Option("order", "The order of the ticket type")]
                                     long order = 0
  ) {
    const string logMessage = $"[{nameof(TicketTypeSlashCommands)}]{nameof(CreateTicketType)}({{name}},{{emoji}},{{description}},{{order}},{{embedMessageTitle}},{{embedMessageContent}})";

    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await TicketType.ProcessCreateTicketTypeAsync(name,
                                                    emoji,
                                                    description,
                                                    order,
                                                    embedMessageTitle,
                                                    embedMessageContent);
      await ctx.EditResponseAsync(Webhooks.Success(Texts.TICKET_TYPE_CREATED, string.Format(Texts.TICKET_TYPE_CREATED_DESCRIPTION, name)));
      Log.Information(logMessage,
                      name,
                      emoji,
                      description,
                      order,
                      embedMessageTitle,
                      embedMessageContent);
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  name,
                  emoji,
                  description,
                  order,
                  embedMessageTitle,
                  embedMessageContent);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                name,
                emoji,
                description,
                order,
                embedMessageTitle,
                embedMessageContent);
    }
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
                                     DiscordEmoji emoji,
                                     [Option("description", "The description of the ticket type")]
                                     string? description = null,
                                     [Option("order", "The order of the ticket type")]
                                     long order = 0
  ) {
    const string logMessage = $"[{nameof(TicketTypeSlashCommands)}]{nameof(UpdateTicketType)}({{name}},{{emoji}},{{description}},{{order}},{{embedMessageTitle}},{{embedMessageContent}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    try {
      var ticketType = await TicketType.GetAsync(name);
      await ticketType.ProcessUpdateTicketTypeAsync(name,
                                                    emoji,
                                                    description,
                                                    order,
                                                    embedMessageTitle,
                                                    embedMessageContent);
      await ctx.EditResponseAsync(Webhooks.Success(Texts.TICKET_TYPE_UPDATED, string.Format(Texts.TICKET_TYPE_UPDATED_DESCRIPTION, name)));
      Log.Information(logMessage,
                      name,
                      emoji,
                      description,
                      order,
                      embedMessageTitle,
                      embedMessageContent);
    }

    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  name,
                  emoji,
                  description,
                  order,
                  embedMessageTitle,
                  embedMessageContent);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                name,
                emoji,
                description,
                order,
                embedMessageTitle,
                embedMessageContent);
    }
  }

  [SlashCommand("delete", "Delete a ticket type")]
  public async Task DeleteTicketType(InteractionContext ctx,
                                     [Option("name", "The name of the ticket type")] [Autocomplete(typeof(TicketTypeProvider))]
                                     string name
  ) {
    const string logMessage = $"[{nameof(TicketTypeSlashCommands)}]{nameof(DeleteTicketType)}({{name}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketType = await TicketType.GetAsync(name);
      await ticketType.ProcessRemoveAsync();
      await ctx.EditResponseAsync(Webhooks.Error(Texts.TICKET_TYPE_DELETED, string.Format(Texts.TICKET_TYPE_DELETED_DESCRIPTION, name)));
      Log.Information(logMessage, name);
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, name);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, name);
    }
  }

  [SlashCommand("list", "List all ticket types")]
  public async Task ListTicketTypes(InteractionContext ctx) {
    const string logMessage = $"[{nameof(TicketTypeSlashCommands)}]{nameof(ListTicketTypes)}()";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    try {
      var ticketTypes = await TicketType.GetAllAsync();
      if (ticketTypes.Count == 0) {
        await ctx.EditResponseAsync(Webhooks.Info(Texts.TICKET_TYPES, Texts.NO_TICKET_TYPES_FOUND));
      }
      else {
        await ctx.EditResponseAsync(Webhooks.Info(Texts.TICKET_TYPES,
                                                  string.Join(Environment.NewLine, ticketTypes.Select(x => $"`{x.Name}` - {x.Description}"))));
      }

      Log.Information(logMessage);
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage);
    }
  }
}