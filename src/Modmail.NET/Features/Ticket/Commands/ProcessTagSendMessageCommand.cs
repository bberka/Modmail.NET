using DSharpPlus.Entities;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessTagSendMessageCommand(
	Guid TicketId,
	Guid TagId,
	DiscordUser ModUser,
	DiscordChannel Channel,
	DiscordGuild Guild
) : IRequest;