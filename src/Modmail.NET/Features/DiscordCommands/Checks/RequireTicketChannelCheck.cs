using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Language;

namespace Modmail.NET.Features.DiscordCommands.Checks;

public class RequireTicketChannelCheck : IContextCheck<RequireTicketChannelAttribute>
{
	public async ValueTask<string?> ExecuteCheckAsync(RequireTicketChannelAttribute attribute, CommandContext context) {
		var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(context.Channel.Topic);
		if (ticketId != Guid.Empty) return null;

		return await Task.FromResult(Lang.ThisCommandCanOnlyBeUsedInTicketChannel.Translate());
	}
}