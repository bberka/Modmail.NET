using MediatR;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketListByTypeQuery(Guid TicketTypeId) : IRequest<List<Database.Entities.Ticket>>;