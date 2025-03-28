using Modmail.NET.Entities;

namespace Modmail.NET.Models.Dto;

public sealed record MetricDto
{
  public required Statistic Statistic { get; init; }
  public required List<bool> TotalTickets { get; init; }
  public required int ActiveTickets { get; init; }
  public required int ClosedTickets { get; init; }
  public required int TotalMessages { get; init; }
  public required int Teams { get; init; }
  public required int Blacklist { get; init; }
  public required int TicketTypes { get; init; }
  public required int TeamRoleCount { get; init; }
  public required int TeamUserCount { get; init; }
  public required ChartItemDto<DateTime, int>[] OpenedTicketsChartDataArray { get; init; }
  public required ChartItemDto<DateTime, int>[] ClosedTicketsChartDataArray { get; init; }
  public required ChartItemDto<DateTime, int>[] ModMessageCountChartDataArray { get; init; }
  public required ChartItemDto<DateTime, int>[] UserMessageCountChartDataArray { get; init; }
  public required ChartItemDto<string, int>[] TicketTypeChartDataArray { get; init; }
  public required ChartItemDto<string, int>[] TicketPriorityChartDataArray { get; init; }
}