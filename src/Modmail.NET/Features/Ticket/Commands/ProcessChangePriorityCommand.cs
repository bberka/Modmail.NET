using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission]
public sealed record ProcessChangePriorityCommand(
	ulong AuthorizedUserId,
	Guid TicketId,
	TicketPriority NewPriority,
	DiscordChannel? TicketChannel = null) : IRequest,
	                                        IPermissionCheck;