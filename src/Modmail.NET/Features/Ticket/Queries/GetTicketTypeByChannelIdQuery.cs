using MediatR;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketTypeByChannelIdQuery(
  ulong ChannelId,
  bool AllowNull = false) : IRequest<TicketType>;