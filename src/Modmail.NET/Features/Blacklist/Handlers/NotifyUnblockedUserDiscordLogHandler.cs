using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class NotifyUnblockedUserDiscordLogHandler : INotificationHandler<NotifyUnblockedUser>
{
	private readonly IMediator _mediator;

	public NotifyUnblockedUserDiscordLogHandler(IMediator mediator) {
		_mediator = mediator;
	}

	public async ValueTask Handle(NotifyUnblockedUser notification, CancellationToken cancellationToken) {
		var authorUser = await _mediator.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.UserUnblocked.Translate())
		            .WithUserAsAuthor(authorUser)
		            .WithColor(ModmailColors.InfoColor)
		            .AddField(Lang.User.Translate(), authorUser.GetMention())
		            .AddField(Lang.UserId.Translate(), notification.AuthorizedUserId.ToString());

		var logChannel = await _mediator.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		await logChannel.SendMessageAsync(embed);
	}
}