namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketClosed(ulong AuthorizedUserId, Database.Entities.Ticket Ticket, bool DontSendFeedbackMessage) : INotification;