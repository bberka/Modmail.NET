using MediatR;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record CheckActiveTicketQuery(Guid TicketId) : IRequest<bool>;