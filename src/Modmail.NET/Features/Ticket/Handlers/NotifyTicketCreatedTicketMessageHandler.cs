using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketCreatedTicketMessageHandler : INotificationHandler<NotifyTicketCreated>
{
	private readonly ISender _sender;
	private readonly ModmailDbContext _dbContext;

	public NotifyTicketCreatedTicketMessageHandler(ISender sender,
	                                               ModmailDbContext dbContext) {
		_sender = sender;
		_dbContext = dbContext;
	}

	public async ValueTask Handle(NotifyTicketCreated notification, CancellationToken cancellationToken) {
		var user = await _sender.Send(new GetDiscordUserInfoQuery(notification.Ticket.OpenerUserId), cancellationToken);

		var guild = await _sender.Send(new GetDiscordMainServerQuery(), cancellationToken);
		var mailChannel = await guild.GetChannelAsync(notification.Ticket.ModMessageChannelId);
		await mailChannel.SendMessageAsync(await GetMessageTicketMessage(notification, user, cancellationToken));
		var botMessage = await mailChannel.SendMessageAsync(MessageReceived(user, notification.Message));

		notification.Message.BotMessageId = botMessage.Id;
		_dbContext.Update(notification.Message);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
	}

	private async ValueTask<DiscordMessageBuilder> GetMessageTicketMessage(NotifyTicketCreated notification, UserInformation user, CancellationToken cancellationToken) {
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.NewTicket.Translate())
		            .WithCustomTimestamp()
		            .WithDescription(Lang.NewTicketDescriptionMessage.Translate())
		            .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
		            .AddField(Lang.User.Translate(), user.GetMention())
		            .AddField(Lang.TicketId.Translate(), notification.Ticket.Id.ToString().ToUpper())
		            .WithColor(ModmailColors.TicketCreatedColor);

		var newTicketMessageBuilder = new DiscordMessageBuilder()
		                              .AddEmbed(embed)
		                              .AddComponents(new DiscordButtonComponent(DiscordButtonStyle.Danger,
		                                                                        UtilInteraction.BuildKey("close_ticket_with_reason", notification.Ticket.Id.ToString()),
		                                                                        Lang.CloseTicket.Translate(),
		                                                                        emoji: new DiscordComponentEmoji("🔒")));

		var permissions = await _sender.Send(new GetUserTeamInformationQuery(), cancellationToken);
		newTicketMessageBuilder.WithContent(UtilMention.GetNewTicketPingText(permissions));
		return newTicketMessageBuilder;
	}

	private static DiscordMessageBuilder MessageReceived(UserInformation userInformation,
	                                                     TicketMessage message) {
		var embed = new DiscordEmbedBuilder()
		            .WithCustomTimestamp()
		            .WithUserAsAuthor(userInformation)
		            .WithColor(ModmailColors.MessageReceivedColor);

		if (!string.IsNullOrEmpty(message.MessageContent)) embed.WithDescription(message.MessageContent);

		var msgBuilder = new DiscordMessageBuilder()
		                 .AddEmbed(embed)
		                 .AddAttachments(message.Attachments.ToArray());
		return msgBuilder;
	}
}