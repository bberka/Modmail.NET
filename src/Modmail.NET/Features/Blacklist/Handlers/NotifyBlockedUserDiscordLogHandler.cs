using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class NotifyBlockedUserDiscordLogHandler : INotificationHandler<NotifyBlockedUser>
{
	private readonly IMediator _mediator;

	public NotifyBlockedUserDiscordLogHandler(IMediator mediator) {
		_mediator = mediator;
	}

	public async Task Handle(NotifyBlockedUser notification, CancellationToken cancellationToken) {
		var authorUser = await _mediator.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.UserBlacklisted.Translate())
		            .WithUserAsAuthor(authorUser)
		            .WithColor(ModmailColors.InfoColor)
		            .AddField(Lang.User.Translate(), authorUser.GetMention())
		            .AddField(Lang.UserId.Translate(), notification.AuthorizedUserId.ToString());

		if (!string.IsNullOrEmpty(notification.Reason)) embed.AddField(Lang.Reason.Translate(), notification.Reason);

		var logChannel = await _mediator.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		await logChannel.SendMessageAsync(embed);
	}
}