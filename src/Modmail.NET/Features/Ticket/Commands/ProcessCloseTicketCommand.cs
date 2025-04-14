using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessCloseTicketCommand(
  Guid TicketId,
  ulong CloserUserId = 0,
  string? CloseReason = null,
  DiscordChannel? ModChatChannel = null,
  bool DontSendFeedbackMessage = false) : IRequest;