using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketMessageSent(Database.Entities.Ticket Ticket, TicketMessage Message) : INotification;