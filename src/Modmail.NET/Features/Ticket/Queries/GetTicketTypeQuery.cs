using MediatR;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketTypeQuery(
  Guid Id,
  bool AllowNull = false) : IRequest<TicketType>;