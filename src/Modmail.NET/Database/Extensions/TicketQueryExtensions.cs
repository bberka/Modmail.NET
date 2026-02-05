using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Extensions;

public static class TicketQueryExtensions
{
    public static IQueryable<Ticket> FilterByModChannelId(this IQueryable<Ticket> query, ulong modChannelId)
    {
        return query.Where(t => t.ModMessageChannelId == modChannelId);
    }

    public static IQueryable<Ticket> FilterByPrivateChannelId(this IQueryable<Ticket> query, ulong privateChannelId)
    {
        return query.Where(t => t.PrivateMessageChannelId == privateChannelId);
    }

    public static IQueryable<Ticket> FilterActive(this IQueryable<Ticket> query)
    {
        return query.Where(t => !t.ClosedDateUtc.HasValue);
    }

    public static IQueryable<Ticket> FilterClosed(this IQueryable<Ticket> query)
    {
        return query.Where(t => t.ClosedDateUtc.HasValue);
    }

    public static IQueryable<Ticket> FilterByOpenerUserId(this IQueryable<Ticket> query, ulong userId)
    {
        return query.Where(t => t.OpenerUserId == userId);
    }

    public static IQueryable<Ticket> FilterByCloserUserId(this IQueryable<Ticket> query, ulong userId)
    {
        return query.Where(t => t.CloserUserId == userId);
    }

    public static IQueryable<Ticket> FilterByAssignedUserId(this IQueryable<Ticket> query, ulong userId)
    {
        return query.Where(t => t.AssignedUserId == userId);
    }

    public static IQueryable<Ticket> FilterWithFeedback(this IQueryable<Ticket> query)
    {
        return query.Where(t => t.FeedbackStar != null);
    }

    public static IQueryable<Ticket> FilterByClosedDate(
        this IQueryable<Ticket> query,
        DateTime startDate,
        DateTime endDate
    )
    {
        return query.Where(t => t.ClosedDateUtc >= startDate && t.ClosedDateUtc <= endDate);
    }

    public static IQueryable<Ticket> FilterByClosedDateStart(this IQueryable<Ticket> query, DateTime startDate)
    {
        return query.Where(t => t.ClosedDateUtc >= startDate);
    }

    public static IQueryable<Ticket> FilterByClosedDateEnd(this IQueryable<Ticket> query, DateTime endDate)
    {
        return query.Where(t => t.ClosedDateUtc <= endDate);
    }

    public static IQueryable<Ticket> FilterByFeedbackStar(this IQueryable<Ticket> query, int feedbackStar)
    {
        return query.Where(t => t.FeedbackStar == feedbackStar);
    }

    public static IQueryable<Ticket> FilterByFeedbackStarRange(
        this IQueryable<Ticket> query,
        int minStar,
        int maxStar
    )
    {
        return query.Where(t => t.FeedbackStar >= minStar && t.FeedbackStar <= maxStar);
    }

    public static IQueryable<Ticket> FilterByLastMessageDate(
        this IQueryable<Ticket> query,
        DateTime startDate,
        DateTime endDate
    )
    {
        return query.Where(t => t.LastMessageDateUtc >= startDate && t.LastMessageDateUtc <= endDate);
    }

    public static IQueryable<Ticket> FilterByLastMessageDateStart(this IQueryable<Ticket> query, DateTime startDate)
    {
        return query.Where(t => t.LastMessageDateUtc >= startDate);
    }

    public static IQueryable<Ticket> FilterByLastMessageDateEnd(this IQueryable<Ticket> query, DateTime endDate)
    {
        return query.Where(t => t.LastMessageDateUtc <= endDate);
    }

    public static IQueryable<Ticket> FilterByTypeId(this IQueryable<Ticket> query, Guid id)
    {
        return query.Where(t => t.TicketTypeId == id);
    }
}