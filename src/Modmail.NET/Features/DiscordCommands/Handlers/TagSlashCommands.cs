using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.DiscordCommands.Helpers;
using Modmail.NET.Features.Tag.Helpers;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Language;
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
	[RequireGuild]
	public async Task Get(SlashCommandContext ctx,
	                      [Parameter("name")] [Description("Tag name")] [SlashAutoCompleteProvider(typeof(TagProvider))]
	                      string name
	) {
		const string logMessage = $"[{nameof(TagSlashCommands)}]{nameof(Get)}({{ContextUserId}},{{TagName}})";
		await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
		try {
			var dbContext = ctx.ServiceProvider.GetRequiredService<ModmailDbContext>();
			var tag = await dbContext.Tags.FilterByTagName(name).FirstOrDefaultAsync();
			if (tag is null) throw new ModmailBotException(Lang.TagNotFound);

			await ctx.EditResponseAsync(TagBotMessages.TagSent(tag));

			var channelTopic = ctx.Channel.Topic;
			var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
			if (ticketId != Guid.Empty) {
				var isActiveTicket = await _sender.Send(new CheckActiveTicketQuery(ticketId));
				if (isActiveTicket) await _sender.Send(new ProcessTagSendMessageCommand(ticketId, tag.Id, ctx.User, ctx.Channel, ctx.Guild!));
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