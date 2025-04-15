using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Tag.Helpers;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Language;
using TicketMessage = Modmail.NET.Database.Entities.TicketMessage;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessTagSendMessageHandler : IRequestHandler<ProcessTagSendMessageCommand>
{
	private readonly ModmailBot _bot;
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessTagSendMessageHandler(ISender sender,
	                                    ModmailBot bot,
	                                    ModmailDbContext dbContext) {
		_sender = sender;
		_bot = bot;
		_dbContext = dbContext;
	}

	public async ValueTask<Unit> Handle(ProcessTagSendMessageCommand request, CancellationToken cancellationToken) {
		ArgumentNullException.ThrowIfNull(request.ModUser);
		ArgumentNullException.ThrowIfNull(request.Channel);
		ArgumentNullException.ThrowIfNull(request.Guild);

		var guildOption = await _sender.Send(new GetOptionQuery(), cancellationToken);

		var ticket = await _dbContext.Tickets.FindAsync([request.TicketId], cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
		ticket.ThrowIfNotOpen();
		ticket.LastMessageDateUtc = UtilDate.GetNow();

		_dbContext.Update(ticket);

		var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
		var tag = await _dbContext.Tags.FindAsync([request.TagId], cancellationToken);
		if (tag is null) throw new ModmailBotException(Lang.TagDoesntExists);

		var ticketMessage = new TicketMessage {
			SenderUserId = request.ModUser.Id,
			MessageDiscordId = 0,
			TicketId = request.TicketId,
			SentByMod = true,
			TagId = tag.Id //TODO: Handle this in transcript UI
		};

		var tagMessage = TagBotMessages.TagReceivedToTicket(tag, request.ModUser, ticket.Anonymous || guildOption.AlwaysAnonymous);
		var botMessage = await privateChannel.SendMessageAsync(tagMessage);

		ticketMessage.BotMessageId = botMessage.Id;
		await _dbContext.AddAsync(ticketMessage, cancellationToken);

		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		return Unit.Value;
	}
}