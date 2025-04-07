namespace Modmail.NET.Features.Metric.Models;

public sealed record ChartItemDto<TCategory, TValue>(TCategory Category, TValue Value);