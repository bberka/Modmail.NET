using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessChangePriorityCommand(
  Guid TicketId,
  ulong ModUserId,
  TicketPriority NewPriority,
  DiscordChannel TicketChannel = null) : IRequest;