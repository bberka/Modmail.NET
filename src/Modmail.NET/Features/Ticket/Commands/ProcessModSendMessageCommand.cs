using DSharpPlus.Entities;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessModSendMessageCommand(
	Guid TicketId,
	DiscordUser ModUser,
	DiscordMessage Message,
	DiscordChannel Channel,
	DiscordGuild Guild
) : IRequest;