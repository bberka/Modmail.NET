using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Metric.Models;
using Modmail.NET.Features.Metric.Queries;

namespace Modmail.NET.Features.Metric.Handlers;

public class GetLatestMetricHandler : IRequestHandler<GetLatestMetricQuery, MetricDto?>
{
	private readonly ModmailDbContext _dbContext;

	public GetLatestMetricHandler(ModmailDbContext dbContext) {
		_dbContext = dbContext;
	}

	public async ValueTask<MetricDto?> Handle(GetLatestMetricQuery request, CancellationToken cancellationToken) {
		var data = await _dbContext.Statistics
		                           .AsNoTracking()
		                           .OrderByDescending(x => x.RegisterDateUtc)
		                           .FirstOrDefaultAsync(cancellationToken);
		if (data is null) return null;

		var totalTickets = await _dbContext.Tickets
		                                   .AsNoTracking()
		                                   .Select(x => x.ClosedDateUtc.HasValue)
		                                   .ToListAsync(cancellationToken);


		var activeTickets = totalTickets.Count(x => !x);
		var closedTickets = totalTickets.Count(x => x);


		var totalMessages = await _dbContext.Messages.CountAsync(cancellationToken);
		var teams = await _dbContext.Teams.CountAsync(cancellationToken);
		var blacklist = await _dbContext.Blacklists.CountAsync(cancellationToken);
		var ticketTypes = await _dbContext.TicketTypes.CountAsync(cancellationToken);

		var teamUserCount = await _dbContext.TeamUsers.CountAsync(cancellationToken);

		const int metricsTakeDay = 14;
		var openTicketsChartDataArray = await _dbContext.Tickets
		                                                .FilterActive()
		                                                .GroupBy(x => x.RegisterDateUtc.Date)
		                                                .OrderByDescending(x => x.Key)
		                                                .Take(metricsTakeDay)
		                                                .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
		                                                .ToArrayAsync(cancellationToken);

		var closedTicketsChartDataArray = await _dbContext.Tickets
		                                                  .FilterClosed()
		                                                  .GroupBy(x => x.ClosedDateUtc!.Value.Date)
		                                                  .OrderByDescending(x => x.Key)
		                                                  .Take(metricsTakeDay)
		                                                  .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
		                                                  .ToArrayAsync(cancellationToken);


		var userMessageCountChartDataArray = await _dbContext.Messages
		                                                     .Where(x => !x.SentByMod)
		                                                     .GroupBy(x => x.RegisterDateUtc.Date)
		                                                     .OrderByDescending(x => x.Key)
		                                                     .Take(metricsTakeDay)
		                                                     .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
		                                                     .ToArrayAsync(cancellationToken);

		var modMessageCountChartDataArray = await _dbContext.Messages
		                                                    .Where(x => x.SentByMod)
		                                                    .GroupBy(x => x.RegisterDateUtc.Date)
		                                                    .OrderByDescending(x => x.Key)
		                                                    .Take(metricsTakeDay)
		                                                    .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
		                                                    .ToArrayAsync(cancellationToken);


		var ticketTypeChartDataArray = await _dbContext.Tickets
		                                               .GroupBy(x => x.TicketTypeId)
		                                               .Select(group => new ChartItemDto<string, int>(group.Key == null
			                                                                                              ? "No Type"
			                                                                                              : group.FirstOrDefault()!.TicketType!.Name,
		                                                                                              group.Count()
		                                                                                             ))
		                                               .ToArrayAsync(cancellationToken);


		var ticketPriorityChartDataArray = await _dbContext.Tickets
		                                                   .GroupBy(x => x.Priority)
		                                                   .Select(group => new ChartItemDto<string, int>(group.Key.ToString(),
		                                                                                                  group.Count()
		                                                                                                 ))
		                                                   .ToArrayAsync(cancellationToken);

		return new MetricDto {
			Statistic = data,
			ActiveTickets = activeTickets,
			ClosedTickets = closedTickets,
			TotalMessages = totalMessages,
			Teams = teams,
			Blacklist = blacklist,
			TicketTypes = ticketTypes,
			TeamUserCount = teamUserCount,
			OpenedTicketsChartDataArray = openTicketsChartDataArray,
			ClosedTicketsChartDataArray = closedTicketsChartDataArray,
			UserMessageCountChartDataArray = userMessageCountChartDataArray,
			ModMessageCountChartDataArray = modMessageCountChartDataArray,
			TicketTypeChartDataArray = ticketTypeChartDataArray,
			TicketPriorityChartDataArray = ticketPriorityChartDataArray
		};
	}
}