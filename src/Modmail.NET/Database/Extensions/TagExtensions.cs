namespace Modmail.NET.Database.Extensions;

public static class TagExtensions
{
    public static IQueryable<Tag> FilterByTagName(this IQueryable<Tag> query, string tagName)
    {
        return query.Where(t => t.Name == tagName);
    }
}