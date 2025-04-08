using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessToggleAnonymousCommand(
  Guid TicketId,
  DiscordChannel TicketChannel = null
) : IRequest;