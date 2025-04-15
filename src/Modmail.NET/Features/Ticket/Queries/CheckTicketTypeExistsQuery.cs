namespace Modmail.NET.Features.Ticket.Queries;

public sealed record CheckTicketTypeExistsQuery(string NameOrKey) : IRequest<bool>;