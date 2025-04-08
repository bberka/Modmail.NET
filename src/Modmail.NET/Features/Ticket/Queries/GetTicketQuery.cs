using MediatR;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketQuery(
  Guid Id,
  bool AllowNull = false,
  bool MustBeOpen = false,
  bool MustBeClosed = false) : IRequest<Database.Entities.Ticket>;