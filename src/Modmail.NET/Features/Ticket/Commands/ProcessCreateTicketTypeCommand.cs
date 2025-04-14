using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessCreateTicketTypeCommand(
  ulong AuthorizedUserId,
  string Name,
  string Emoji,
  string Description,
  long Order,
  string? EmbedMessageTitle,
  string? EmbedMessageContent) : IRequest,
                                 IPermissionCheck;