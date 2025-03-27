using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Models.Dto;

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
                                       .ToListAsync(cancellationToken: cancellationToken);


    var activeTickets = totalTickets.Count(x => !x);
    var closedTickets = totalTickets.Count(x => x);


    var totalMessages = await _dbContext.TicketMessages.CountAsync(cancellationToken: cancellationToken);
    var teams = await _dbContext.GuildTeams.CountAsync(cancellationToken: cancellationToken);
    var blacklist = await _dbContext.TicketBlacklists.CountAsync(cancellationToken: cancellationToken);
    var ticketTypes = await _dbContext.TicketTypes.CountAsync(cancellationToken: cancellationToken);


    var teamMemberData = await _dbContext.GuildTeams.Where(x => x.IsEnabled).SelectMany(x => x.GuildTeamMembers).GroupBy(x => x.Type).ToListAsync(cancellationToken: cancellationToken);
    var teamRoleCount = teamMemberData.Count(x => x.Key == TeamMemberDataType.RoleId);
    var teamUserCount = teamMemberData.Count(x => x.Key == TeamMemberDataType.UserId);

    const int metricsTakeDay = 14;
    var openTicketsChartDataArray = await _dbContext.Tickets
                                                    .GroupBy(x => x.RegisterDateUtc.Date)
                                                    .OrderByDescending(x => x.Key)
                                                    .Take(metricsTakeDay)
                                                    .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                    .ToArrayAsync(cancellationToken: cancellationToken);

    var closedTicketsChartDataArray = await _dbContext.Tickets
                                                      .Where(x => x.ClosedDateUtc.HasValue)
                                                      .GroupBy(x => x.ClosedDateUtc.Value.Date)
                                                      .OrderByDescending(x => x.Key)
                                                      .Take(metricsTakeDay)
                                                      .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                      .ToArrayAsync(cancellationToken: cancellationToken);


    var userMessageCountChartDataArray = await _dbContext.TicketMessages
                                                         .Where(x => !x.SentByMod)
                                                         .GroupBy(x => x.RegisterDateUtc.Date)
                                                         .OrderByDescending(x => x.Key)
                                                         .Take(metricsTakeDay)
                                                         .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                         .ToArrayAsync(cancellationToken: cancellationToken);

    var modMessageCountChartDataArray = await _dbContext.TicketMessages
                                                        .Where(x => x.SentByMod)
                                                        .GroupBy(x => x.RegisterDateUtc.Date)
                                                        .OrderByDescending(x => x.Key)
                                                        .Take(metricsTakeDay)
                                                        .Select(x => new ChartItemDto<DateTime, int>(x.Key, x.Count()))
                                                        .ToArrayAsync(cancellationToken: cancellationToken);


    var ticketTypeChartDataArray = await _dbContext.Tickets
                                                   .GroupBy(x => x.TicketTypeId)
                                                   .Select(group => new ChartItemDto<string, int>(group.Key == null
                                                                                                    ? "No Type"
                                                                                                    : group.FirstOrDefault().TicketType.Name,
                                                                                                  group.Count()
                                                                                                 ))
                                                   .ToArrayAsync(cancellationToken: cancellationToken);


    var ticketPriorityChartDataArray = await _dbContext.Tickets
                                                       .GroupBy(x => x.Priority)
                                                       .Select(group => new ChartItemDto<string, int>(group.Key.ToString(),
                                                                                                      group.Count()
                                                                                                     ))
                                                       .ToArrayAsync(cancellationToken: cancellationToken);

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
      ModMessageCountChartDataArray = userMessageCountChartDataArray,
      TicketTypeChartDataArray = ticketTypeChartDataArray,
      TicketPriorityChartDataArray = ticketPriorityChartDataArray,
    };
  }
}