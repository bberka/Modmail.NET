using MediatR;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketByUserIdQuery(
  ulong UserId,
  bool AllowNull = false,
  bool MustBeOpen = false,
  bool MustBeClosed = false) : IRequest<Database.Entities.Ticket>;