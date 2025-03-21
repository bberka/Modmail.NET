using MediatR;

namespace Modmail.NET.Features.Ticket;

public sealed record GetTicketQuery(
  Guid Id,
  bool AllowNull = false,
  bool MustBeOpen = false,
  bool MustBeClosed = false) : IRequest<Entities.Ticket>;

public sealed record GetTicketByUserIdQuery(
  ulong UserId,
  bool AllowNull = false,
  bool MustBeOpen = false,
  bool MustBeClosed = false) : IRequest<Entities.Ticket>;

public sealed record GetTimedOutTicketListQuery(long TimeoutHours) : IRequest<List<Entities.Ticket>>;

public sealed record GetTicketListByTypeQuery(Guid TicketTypeId) : IRequest<List<Entities.Ticket>>;