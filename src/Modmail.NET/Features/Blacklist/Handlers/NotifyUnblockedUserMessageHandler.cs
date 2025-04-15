using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class NotifyUnblockedUserMessageHandler : INotificationHandler<NotifyUnblockedUser>
{
	private readonly IMediator _mediator;

	public NotifyUnblockedUserMessageHandler(IMediator mediator) {
		_mediator = mediator;
	}

	public async Task Handle(NotifyUnblockedUser notification, CancellationToken cancellationToken) {
		var member = await _mediator.Send(new GetDiscordMemberQuery(notification.UserId), cancellationToken);
		if (member is null)
			//TODO: Log
			return;

		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.YouHaveBeenRemovedFromBlacklist.Translate())
		            .WithDescription(Lang.YouHaveBeenRemovedFromBlacklistDescription.Translate())
		            .WithGuildInfoFooter()
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.SuccessColor);

		await member.SendMessageAsync(embed);
	}
}