using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Extensions;

public static class BlacklistExtensions
{
    public static IQueryable<Blacklist> FilterByUserId(this IQueryable<Blacklist> query, ulong userId)
    {
        return query.Where(t => t.UserId == userId);
    }

    public static IQueryable<Blacklist> FilterByReason(this IQueryable<Blacklist> query, string reason)
    {
        return query.Where(t => t.Reason.Contains(reason));
    }
}