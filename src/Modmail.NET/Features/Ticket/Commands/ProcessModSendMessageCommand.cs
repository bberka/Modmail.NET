using DSharpPlus.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission]
public sealed record ProcessModSendMessageCommand(
    ulong AuthorizedUserId,
    Guid TicketId,
    DiscordMessage Message) : IRequest, IPermissionCheck;