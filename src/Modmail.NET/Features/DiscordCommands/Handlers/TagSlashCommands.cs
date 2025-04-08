using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.DiscordCommands.Helpers;
using Modmail.NET.Features.Tag.Helpers;
using Modmail.NET.Features.Tag.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Queries;
using Serilog;

namespace Modmail.NET.Features.DiscordCommands.Handlers;

public class TagSlashCommands
{
  private readonly ISender _sender;

  public TagSlashCommands(ISender sender) {
    _sender = sender;
  }

  [Command("tag")]
  [Description("Get tag content")]
  [PerformanceLoggerAspect]
  [UpdateUserInformation]
  public async Task Get(SlashCommandContext ctx,
                        [Parameter("name")] [Description("Tag name")] [SlashAutoCompleteProvider(typeof(TagProvider))]
                        string name
  ) {
    const string logMessage = $"[{nameof(TagSlashCommands)}]{nameof(Get)}({{ContextUserId}},{{TagName}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
    try {
      var tag = await _sender.Send(new GetTagByNameQuery(name));

      await ctx.EditResponseAsync(TagBotMessages.TagSent(tag));

      var channelTopic = ctx.Channel.Topic;
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
      if (ticketId != Guid.Empty) {
        var isActiveTicket = await _sender.Send(new CheckActiveTicketQuery(ticketId));
        if (isActiveTicket) await _sender.Send(new ProcessTagSendMessageCommand(ticketId, tag.Id, ctx.User, ctx.Channel, ctx.Guild));
      }

      Log.Information(logMessage, ctx.User.Id, name);
    }
    catch (ModmailBotException ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, name);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, name);
    }
  }
}