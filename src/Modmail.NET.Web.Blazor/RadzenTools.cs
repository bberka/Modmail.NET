using System.Linq.Dynamic.Core;
using Modmail.NET.Common.Exceptions;
using Radzen;

namespace Modmail.NET.Web.Blazor;

public static class RadzenTools
{
    public static IQueryable<T> ApplyDataGridFilter<T>(this IQueryable<T> queryable, LoadDataArgs? args = null)
    {
        if (args is null) return queryable;

        if (args.Filter is not null) queryable = queryable.Where(args.Filter);

        var sorts = args.Sorts?.ToArray();
        if (sorts is not null && sorts.Length > 0)
        {
            var firstSort = sorts.First();
            queryable = firstSort.SortOrder == SortOrder.Ascending
                ? queryable.OrderBy(x => firstSort.Property)
                : queryable.OrderByDescending(x => firstSort.Property);
        }


        return queryable;
    }

    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, LoadDataArgs args)
    {
        var skip = Math.Max(0, args.Skip ?? 0);
        var take = Math.Max(0, args.Top ?? 10);
        return queryable.Skip(skip)
            .Take(take);
    }

    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> queryable,
        int page,
        int pageSize
    )
    {
        var skip = Math.Max(0, page - 1) * pageSize;
        return queryable.Skip(skip)
            .Take(pageSize);
    }

    public static void NotifyException<T>(
        this T exception,
        NotificationService service,
        bool showExceptionMessage = false
    ) where T : Exception
    {
        if (exception is ModmailBotException botException)
        {
            service.Notify(NotificationSeverity.Warning, "Failed", botException.TitleMessage);
            return;
        }

        service.Notify(NotificationSeverity.Error, "Error", "An exception occurred, please check logs.");
    }
}