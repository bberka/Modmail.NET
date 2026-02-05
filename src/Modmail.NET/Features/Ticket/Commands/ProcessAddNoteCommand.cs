using MediatR;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessAddNoteCommand(
  Guid TicketId,
  ulong UserId,
  string Note
) : IRequest;