using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Extensions;

public static class TagExtensions
{
  public static IQueryable<Tag> FilterByTagName(this IQueryable<Tag> query, string tagName) {
    return query.Where(t => t.Name == tagName);
  }

  public static IQueryable<Tag> FilterByContent(this IQueryable<Tag> query, string content) {
    return query.Where(t => t.Content == content);
  }

  public static IQueryable<Tag> FilterByTitle(this IQueryable<Tag> query, string title) {
    return query.Where(t => t.Title == title);
  }
}