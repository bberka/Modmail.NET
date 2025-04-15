namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketAnonymousToggled(ulong AuthorizedUserId, Database.Entities.Ticket Ticket) : INotification;