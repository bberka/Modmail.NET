using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class NotifyUnblockedUserMessageHandler : INotificationHandler<NotifyUnblockedUser>
{
	private readonly IMediator _mediator;

	public NotifyUnblockedUserMessageHandler(IMediator mediator) {
		_mediator = mediator;
	}

	public async ValueTask Handle(NotifyUnblockedUser notification, CancellationToken cancellationToken) {
		var member = await _mediator.Send(new GetDiscordMemberQuery(notification.UserId), cancellationToken);
		if (member is null)
			//TODO: Log
			return;
		var option = await _mediator.Send(new GetOptionQuery(), cancellationToken);

		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.YouHaveBeenRemovedFromBlacklist.Translate())
		            .WithDescription(Lang.YouHaveBeenRemovedFromBlacklistDescription.Translate())
		            .WithServerInfoFooter(option)
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.SuccessColor);

		await member.SendMessageAsync(embed);
	}
}