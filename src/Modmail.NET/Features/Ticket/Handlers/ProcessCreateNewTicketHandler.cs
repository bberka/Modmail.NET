using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.Ticket.Jobs;
using Modmail.NET.Features.Ticket.Services;
using Modmail.NET.Features.Ticket.Static;
using TicketMessage = Modmail.NET.Database.Entities.TicketMessage;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessCreateNewTicketHandler : IRequestHandler<ProcessCreateNewTicketCommand>
{
	private readonly TicketAttachmentDownloadService _attachmentDownloadService;
	private readonly ModmailBot _bot;
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;
	private readonly TicketTypeSelectionTimeoutJob _ticketTypeSelectionTimeoutJob;

	public ProcessCreateNewTicketHandler(ISender sender,
	                                     ModmailBot bot,
	                                     ModmailDbContext dbContext,
	                                     TicketTypeSelectionTimeoutJob ticketTypeSelectionTimeoutJob,
	                                     TicketAttachmentDownloadService attachmentDownloadService) {
		_sender = sender;
		_bot = bot;
		_dbContext = dbContext;
		_ticketTypeSelectionTimeoutJob = ticketTypeSelectionTimeoutJob;
		_attachmentDownloadService = attachmentDownloadService;
	}

	public async ValueTask<Unit> Handle(ProcessCreateNewTicketCommand request, CancellationToken cancellationToken) {
		var guildOption = await _sender.Send(new GetOptionQuery(), cancellationToken);

		var guild = await _sender.Send(new GetDiscordMainServerQuery(), cancellationToken);
		//make new privateChannel
		var channelName = string.Format(TicketConstants.TicketNameTemplate, request.User.Username.Trim());
		var category = await _bot.Client.GetChannelAsync(guildOption.CategoryId);

		var ticketId = Guid.CreateVersion7();

		var permissions = await _sender.Send(new GetUserTeamInformationQuery(), cancellationToken);
		var members = await guild.GetAllMembersAsync(cancellationToken).ToListAsync(cancellationToken);

		var modMemberListForOverwrites = UtilPermission.ParsePermissionInfo(permissions, members);
		var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, modMemberListForOverwrites);
		var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);

		var member = await _sender.Send(new GetDiscordMemberQuery(request.User.Id), cancellationToken);
		if (member is null) return Unit.Value;

		var msg = TicketBotMessages.Ticket.NewTicket(member, ticketId);
		msg.WithContent(UtilMention.GetNewTicketPingText(permissions));

		var ticketMessage = TicketMessage.MapFrom(ticketId, request.Message, false);

		var ticket = new Database.Entities.Ticket {
			OpenerUserId = request.User.Id,
			ModMessageChannelId = mailChannel.Id,
			RegisterDateUtc = UtilDate.GetNow(),
			PrivateMessageChannelId = request.PrivateChannel.Id,
			InitialMessageId = request.Message.Id,
			Priority = TicketPriority.Normal,
			LastMessageDateUtc = UtilDate.GetNow(),
			Id = ticketId,
			Anonymous = guildOption.AlwaysAnonymous,
			IsForcedClosed = false,
			CloseReason = null,
			FeedbackMessage = null,
			FeedbackStar = null,
			CloserUserId = null,
			ClosedDateUtc = null,
			TicketTypeId = null,
			Messages = [ticketMessage],
			BotTicketCreatedMessageInDmId = 0
		};


		var ticketTypes = await _dbContext.TicketTypes.ToListAsync(cancellationToken);

		var ticketCreatedMessage = await request.PrivateChannel.SendMessageAsync(TicketBotMessages.User.YouHaveCreatedNewTicket(guild,
		                                                                                                                        guildOption,
		                                                                                                                        ticketTypes,
		                                                                                                                        ticketId));

		_ticketTypeSelectionTimeoutJob.AddMessage(ticketCreatedMessage);


		ticket.BotTicketCreatedMessageInDmId = ticketCreatedMessage.Id;

		_dbContext.Add(ticket);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();

		foreach (var attachment in ticketMessage.Attachments)
			await _attachmentDownloadService.Handle(attachment.Id, attachment.Url, Path.GetExtension(attachment.FileName));

		await mailChannel.SendMessageAsync(msg);
		var botMessage = await mailChannel.SendMessageAsync(TicketBotMessages.Ticket.MessageReceived(request.Message, ticketMessage.Attachments.ToArray()));
		ticketMessage.BotMessageId = botMessage.Id;

		var newTicketCreatedLog = LogBotMessages.NewTicketCreated(request.Message, ticket.Id);
		var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		await logChannel.SendMessageAsync(newTicketCreatedLog);

		await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(TicketConstants.ProcessedReactionDiscordEmojiUnicode));
		return Unit.Value;
	}
}