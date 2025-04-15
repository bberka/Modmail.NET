using Modmail.NET.Abstract;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission]
public sealed record ProcessCloseTicketCommand(
	ulong AuthorizedUserId,
	Guid TicketId,
	string? CloseReason = null,
	bool DontSendFeedbackMessage = false) : IRequest,
	                                        IPermissionCheck;