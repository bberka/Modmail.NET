using DSharpPlus.Entities;

namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketCreated(Guid TicketId, DiscordUser User, DiscordMessage Message, DiscordChannel PrivateChannel) : INotification;