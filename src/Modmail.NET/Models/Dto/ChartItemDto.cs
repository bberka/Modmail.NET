namespace Modmail.NET.Models.Dto;

public sealed record ChartItemDto<TCategory,TValue>(TCategory Category, TValue Value);