namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessChangeTicketTypeCommand(
	ulong AuthorizedUserId,
	Guid TicketId,
	string Type
) : IRequest;