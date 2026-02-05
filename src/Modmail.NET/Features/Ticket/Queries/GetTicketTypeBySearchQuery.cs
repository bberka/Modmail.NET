using MediatR;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketTypeBySearchQuery(
  string NameOrKey,
  bool AllowNull = false) : IRequest<TicketType>;