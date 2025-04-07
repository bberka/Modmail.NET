using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Ticket.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessCreateTicketTypeCommand(
  ulong AuthorizedUserId,
  string Name,
  string Emoji,
  string Description,
  long Order,
  string EmbedMessageTitle,
  string EmbedMessageContent) : IRequest;