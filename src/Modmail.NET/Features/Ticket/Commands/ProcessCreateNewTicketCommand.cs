using DSharpPlus.Entities;

namespace Modmail.NET.Features.Ticket.Commands;

public sealed record ProcessCreateNewTicketCommand(
	DiscordUser User,
	DiscordChannel PrivateChannel,
	DiscordMessage Message) : IRequest;