using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Metric.Models;

public sealed record MetricDto
{
	public required Statistic? Statistic { get; init; }
	public required int ActiveTickets { get; init; }
	public required int ClosedTickets { get; init; }
	public required int TotalMessages { get; init; }
	public required int Teams { get; init; }
	public required int Blacklist { get; init; }
	public required int TicketTypes { get; init; }
	public required int TeamUserCount { get; init; }
	public required int TagCount { get; init; }
	public required ChartItemDto<DateTime, int>[] OpenedTicketsChartDataArray { get; init; }
	public required ChartItemDto<DateTime, int>[] ClosedTicketsChartDataArray { get; init; }
	public required ChartItemDto<DateTime, int>[] ModMessageCountChartDataArray { get; init; }
	public required ChartItemDto<DateTime, int>[] UserMessageCountChartDataArray { get; init; }
	public required ChartItemDto<string, int>[] TicketTypeChartDataArray { get; init; }
	public required ChartItemDto<string, int>[] TicketPriorityChartDataArray { get; init; }

	public static MetricDto Default => new() {
		Statistic = new Statistic {
			AvgResponseTimeSeconds = 0,
			AvgTicketsClosedPerDay = 0,
			AvgTicketsOpenedPerDay = 0,
			AvgTicketClosedSeconds = 0,
			FastestClosedTicketSeconds = 0,
			SlowestClosedTicketSeconds = 0
		},
		ActiveTickets = 0,
		ClosedTickets = 0,
		TotalMessages = 0,
		Teams = 0,
		Blacklist = 0,
		TicketTypes = 0,
		TeamUserCount = 0,
		TagCount = 0,
		OpenedTicketsChartDataArray = [],
		ClosedTicketsChartDataArray = [],
		ModMessageCountChartDataArray = [],
		UserMessageCountChartDataArray = [],
		TicketTypeChartDataArray = [],
		TicketPriorityChartDataArray = []
	};
}