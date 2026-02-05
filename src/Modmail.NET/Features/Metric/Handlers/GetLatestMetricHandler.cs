using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Metric.Models;
using Modmail.NET.Features.Metric.Queries;
using Modmail.NET.Features.Teams.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Metric.Handlers;

public class GetLatestMetricHandler : IRequestHandler<GetLatestMetricQuery, MetricDto>
{
  private readonly ModmailDbContext _dbContext;

  public GetLatestMetricHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<MetricDto> Handle(GetLatestMetricQuery request, CancellationToken cancellationToken) {
    var data = await _dbContext.Statistics
                               .AsNoTracking()
                               .OrderByDescending(x => x.RegisterDateUtc)
                               .FirstOrDefaultAsync(cancellationToken);
    if (!request.AllowNull && data is null) throw new NotFoundException(LangKeys.Statistic);

    var totalTickets = await _dbContext.Tickets
                                       .AsNoTracking()
                                       .Select(x => x.ClosedDateUtc.HasValue)
                                       .ToListAsync(cancellationToken);


    var activeTickets = totalTickets.Count(x => !x);
    var closedTickets = totalTickets.Count(x => x);


    var totalMessages = await _dbContext.TicketMessages.CountAsync(cancellationToken);
    var teams = await _dbContext.GuildTeams.CountAsync(cancellationToken);
    var blacklist = await _dbContext.TicketBlacklists.CountAsync(cancellationToken);
    var ticketTypes = await _dbContext.TicketTypes.CountAsync(cancellationToken);


    var teamMemberData = await _dbContext.GuildTeams.Where(x => x.IsEnabled).SelectMany(x => x.GuildTeamMembers).GroupBy(x => x.Type).ToListAsync(cancellationToken);
    var teamRoleCount = teamMemberData.Count(x => x.Key == TeamMemberDataType.RoleId);
    var teamUserCount = teamMemberData.Count(x => x.Key == TeamMemberDataType.UserId);

    const int metricsTakeDay = 14;
    var openTicketsChartDataArray = await _dbContext.Tickets
                                                    .GroupBy(x => x.RegisterDateUtc.Date)
                                                    .OrderByDescending(x => x.Key)
                                                    .Take(metricsTakeDay)
                                                    .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                    .ToArrayAsync(cancellationToken);

    var closedTicketsChartDataArray = await _dbContext.Tickets
                                                      .Where(x => x.ClosedDateUtc.HasValue)
                                                      .GroupBy(x => x.ClosedDateUtc.Value.Date)
                                                      .OrderByDescending(x => x.Key)
                                                      .Take(metricsTakeDay)
                                                      .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                      .ToArrayAsync(cancellationToken);


    var userMessageCountChartDataArray = await _dbContext.TicketMessages
                                                         .Where(x => !x.SentByMod)
                                                         .GroupBy(x => x.RegisterDateUtc.Date)
                                                         .OrderByDescending(x => x.Key)
                                                         .Take(metricsTakeDay)
                                                         .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                         .ToArrayAsync(cancellationToken);

    var modMessageCountChartDataArray = await _dbContext.TicketMessages
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
                                                                                                    : group.FirstOrDefault().TicketType.Name,
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
      TotalTickets = totalTickets,
      ActiveTickets = activeTickets,
      ClosedTickets = closedTickets,
      TotalMessages = totalMessages,
      Teams = teams,
      Blacklist = blacklist,
      TicketTypes = ticketTypes,
      TeamRoleCount = teamRoleCount,
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