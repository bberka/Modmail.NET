using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketTypeChanged(ulong AuthorizedUserId, Database.Entities.Ticket Ticket, TicketType? TicketType) : INotification;