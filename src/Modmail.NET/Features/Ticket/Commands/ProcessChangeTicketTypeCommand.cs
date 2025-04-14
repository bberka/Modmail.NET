using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessChangeTicketTypeCommand(
  Guid TicketId,
  string Type,
  DiscordChannel? TicketChannel = null,
  DiscordChannel? PrivateChannel = null,
  DiscordMessage? PrivateMessageWithComponent = null,
  ulong UserId = 0
) : IRequest;