using MediatR;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Queries;

public sealed record GetTicketTypeListQuery(bool OnlyActive = false) : IRequest<List<TicketType>>;