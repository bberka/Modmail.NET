using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Notifications;

public sealed record NotifyTicketNoteAdded(ulong AuthorizedUserId, Database.Entities.Ticket Ticket, TicketNote Note) : INotification;