using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.Ticket.Static;
using TicketMessage = Modmail.NET.Database.Entities.TicketMessage;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessCreateNewTicketHandler : IRequestHandler<ProcessCreateNewTicketCommand>
{
	private readonly ModmailBot _bot;
	private readonly ModmailDbContext _dbContext;
	private readonly IMediator _mediator;

	public ProcessCreateNewTicketHandler(IMediator mediator,
	                                     ModmailBot bot,
	                                     ModmailDbContext dbContext) {
		_mediator = mediator;
		_bot = bot;
		_dbContext = dbContext;
	}

	public async ValueTask<Unit> Handle(ProcessCreateNewTicketCommand request, CancellationToken cancellationToken) {
		var option = await _mediator.Send(new GetOptionQuery(), cancellationToken);
		var guild = await _mediator.Send(new GetDiscordMainServerQuery(), cancellationToken);
		var channelName = string.Format(TicketConstants.TicketNameTemplate, request.User.Username.Trim());
		var ticketId = Guid.CreateVersion7();
		var members = await guild.GetAllMembersAsync(cancellationToken).ToListAsync(cancellationToken);
		var permissions = await _mediator.Send(new GetUserTeamInformationQuery(), cancellationToken);
		var modMemberListForOverwrites = UtilPermission.ParsePermissionInfo(permissions, members);
		var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, modMemberListForOverwrites);
		var category = await _bot.Client.GetChannelAsync(option.CategoryId);
		var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);
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
			Anonymous = false,
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
		_dbContext.Add(ticket);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(TicketConstants.ProcessedReactionDiscordEmojiUnicode));
		await _mediator.Publish(new NotifyTicketCreated(ticket, ticketMessage), cancellationToken);
		return Unit.Value;
	}
}