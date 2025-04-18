using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessUserSentMessageCommand(
  Guid TicketId,
  DiscordMessage Message,
  DiscordChannel PrivateChannel = null) : IRequest;