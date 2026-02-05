using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Extensions;

public static class TicketTypeQueryExtensions
{
    public static IQueryable<TicketType> FilterByNameOrKey(this IQueryable<TicketType> query, string nameOrKey)
    {
        return query.Where(t => t.Name == nameOrKey || t.Key == nameOrKey);
    }

    public static IQueryable<TicketType> FilterByName(this IQueryable<TicketType> query, string name)
    {
        return query.Where(t => t.Name == name);
    }
}