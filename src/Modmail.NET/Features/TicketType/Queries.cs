using MediatR;

namespace Modmail.NET.Features.TicketType;

public sealed record GetTicketTypeQuery(
  Guid Id,
  bool AllowNull = false) : IRequest<Entities.TicketType>;

public sealed record GetTicketTypeBySearchQuery(
  string NameOrKey,
  bool AllowNull = false) : IRequest<Entities.TicketType>;

public sealed record GetTicketTypeByChannelIdQuery(
  ulong ChannelId,
  bool AllowNull = false) : IRequest<Entities.TicketType>;

public sealed record GetTicketTypeListQuery(bool OnlyActive = false) : IRequest<List<Entities.TicketType>>;

public sealed record CheckTicketTypeExistsQuery(string NameOrKey) : IRequest<bool>;