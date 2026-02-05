using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessAddFeedbackCommand(
  Guid TicketId,
  int StarCount,
  string TextInput,
  DiscordMessage FeedbackMessage
) : IRequest;