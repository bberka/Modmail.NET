using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessAddFeedbackHandler : IRequestHandler<ProcessAddFeedbackCommand>
{
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessAddFeedbackHandler(ISender sender,
	                                 ModmailDbContext dbContext,
	                                 ModmailBot bot) {
		_sender = sender;
		_dbContext = dbContext;
	}

	public async ValueTask<Unit> Handle(ProcessAddFeedbackCommand request, CancellationToken cancellationToken) {
		if (request.StarCount is < 1 or > 5) throw new InvalidOperationException("Star count must be between 1 and 5");
		if (string.IsNullOrEmpty(request.TextInput)) throw new InvalidOperationException("Feedback messageContent is empty");
		if (request.FeedbackMessage is null) throw new InvalidOperationException("Feedback messageContent is null");

		var guildOption = await _sender.Send(new GetOptionQuery(), cancellationToken);
		if (!guildOption.TakeFeedbackAfterClosing) throw new InvalidOperationException("Feedback is not enabled for this guild: " + guildOption.ServerId);

		var ticket = await _dbContext.Tickets.FindAsync([request.TicketId], cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
		ticket.ThrowIfNotClosed();
		if (ticket.FeedbackStar.HasValue) throw new ModmailBotException(Lang.FeedbackAlreadySubmitted);

		ticket.FeedbackStar = request.StarCount;
		ticket.FeedbackMessage = request.TextInput;
		_dbContext.Update(ticket);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();

		await request.FeedbackMessage.ModifyAsync(x => {
			x.Clear();
			x.AddEmbed(TicketBotMessages.User.FeedbackReceivedUpdateMessage(ticket));
		});
		return Unit.Value;
	}
}