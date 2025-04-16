namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessTagSendMessageCommand(
	ulong UserId,
	Guid TicketId,
	Guid TagId,
	ulong ChannelId
) : IRequest;