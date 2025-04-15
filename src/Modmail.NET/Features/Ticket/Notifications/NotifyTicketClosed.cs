namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketClosed(Database.Entities.Ticket Ticket, bool DontSendFeedbackMessage) : INotification;