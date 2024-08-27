using Radzen;
using System.Linq.Dynamic.Core;


namespace Modmail.NET.Web.Blazor;

public static class RadzenTools
{
  public static IQueryable<T> ApplyDataGridFilter<T>(this IQueryable<T> queryable, LoadDataArgs? args = null) {
    if (args is null) return queryable;

    // if (args.Filter is not null) {
    // }

    // var expression = Expression.Constant(true);
    // foreach (var filter in args.Filters) {
    //   var filterValue = filter.FilterValue;
    //   var filterType = filter.FilterOperator;
    //   var filterField = filter.;
    //   
    // }

    foreach (var sort in args.Sorts) {
      var sortField = sort.Property;
      if (string.IsNullOrEmpty(sort.Property)) continue;
      var sortDir = sort.SortOrder;
      queryable = queryable.OrderBy(sortField + (sortDir == SortOrder.Ascending
                                                   ? ""
                                                   : " descending"));
    }


    return queryable;
  }

  public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, LoadDataArgs args) {
    var skip = Math.Max(0, args.Skip ?? 0);
    var take = Math.Max(0, args.Top ?? 10);
    return queryable.Skip(skip).Take(take);
  }

  public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, int page, int pageSize) {
    var skip = Math.Max(0, page - 1) * pageSize;
    return queryable.Skip(skip).Take(pageSize);
  }
}