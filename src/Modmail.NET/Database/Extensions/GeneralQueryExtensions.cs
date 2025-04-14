using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Extensions;

public static class GeneralQueryExtensions
{
  public static IQueryable<T> FilterById<T>(this IQueryable<T> query, Guid id) where T : IGuidId {
    return query.Where(t => t.Id == id);
  }

  public static IQueryable<T> FilterByRegisterDate<T>(this IQueryable<T> query, DateTime startDate, DateTime endDate) where T : IRegisterDateUtc {
    return query.Where(t => t.RegisterDateUtc >= startDate && t.RegisterDateUtc <= endDate);
  }

  public static IQueryable<T> FilterByRegisterDateStart<T>(this IQueryable<T> query, DateTime startDate) where T : IRegisterDateUtc {
    return query.Where(t => t.RegisterDateUtc >= startDate);
  }

  public static IQueryable<T> FilterByRegisterDateEnd<T>(this IQueryable<T> query, DateTime endDate) where T : IRegisterDateUtc {
    return query.Where(t => t.RegisterDateUtc <= endDate);
  }

  public static IQueryable<T> FilterByUpdateDate<T>(this IQueryable<T> query, DateTime startDate, DateTime endDate) where T : IUpdateDateUtc {
    return query.Where(t => t.UpdateDateUtc >= startDate && t.UpdateDateUtc <= endDate);
  }

  public static IQueryable<T> FilterByUpdateDateStart<T>(this IQueryable<T> query, DateTime startDate) where T : IUpdateDateUtc {
    return query.Where(t => t.UpdateDateUtc >= startDate);
  }

  public static IQueryable<T> FilterByUpdateDateEnd<T>(this IQueryable<T> query, DateTime endDate) where T : IUpdateDateUtc {
    return query.Where(t => t.UpdateDateUtc <= endDate);
  }
}