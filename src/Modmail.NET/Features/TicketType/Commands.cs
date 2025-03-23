using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;

namespace Modmail.NET.Features.TicketType;

[PermissionCheck(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessUpdateTicketTypeCommand(ulong AuthorizedUserId, Entities.TicketType TicketType) : IRequest;


[PermissionCheck(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessCreateTicketTypeCommand(
  ulong AuthorizedUserId,
  string Name,
  string Emoji,
  string Description,
  long Order,
  string EmbedMessageTitle,
  string EmbedMessageContent) : IRequest;


[PermissionCheck(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessRemoveTicketTypeCommand(ulong AuthorizedUserId,Guid Id) : IRequest<Entities.TicketType>,
                                                                                     IPermissionCheck;