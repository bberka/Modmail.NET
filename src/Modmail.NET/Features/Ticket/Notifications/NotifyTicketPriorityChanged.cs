using MediatR;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketPriorityChanged(ulong AuthorizedUserId, Database.Entities.Ticket Ticket, TicketPriority OldPriority, TicketPriority NewPriority) : INotification;