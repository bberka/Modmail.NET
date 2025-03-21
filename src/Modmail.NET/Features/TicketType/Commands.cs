using MediatR;

namespace Modmail.NET.Features.TicketType;

public sealed record ProcessUpdateTicketTypeCommand(Entities.TicketType TicketType) : IRequest;

public sealed record ProcessCreateTicketTypeCommand(
  string Name,
  string Emoji,
  string Description,
  long Order,
  string EmbedMessageTitle,
  string EmbedMessageContent) : IRequest;

public sealed record ProcessRemoveTicketTypeCommand(Guid Id) : IRequest<Entities.TicketType>;