using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class NotifyBlockedUserMessageHandler : INotificationHandler<NotifyBlockedUser>
{
	private readonly IMediator _mediator;

	public NotifyBlockedUserMessageHandler(IMediator mediator) {
		_mediator = mediator;
	}

	public async ValueTask Handle(NotifyBlockedUser notification, CancellationToken cancellationToken) {
		var member = await _mediator.Send(new GetDiscordMemberQuery(notification.UserId), cancellationToken);
		if (member is null)
			//TODO: Log
			return;

		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.YouHaveBeenBlacklisted.Translate())
		            .WithDescription(Lang.YouHaveBeenBlacklistedDescription.Translate())
		            .WithGuildInfoFooter()
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.ErrorColor);

		if (!string.IsNullOrEmpty(notification.Reason)) embed.AddField(Lang.Reason.Translate(), notification.Reason);

		await member.SendMessageAsync(embed);
	}
}