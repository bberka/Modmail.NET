using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission]
public sealed record ProcessToggleAnonymousCommand(
	ulong AuthorizedUserId,
	Guid TicketId
) : IRequest,
    IPermissionCheck;