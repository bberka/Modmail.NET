using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Metric.Static;
using Modmail.NET.Features.Server.Queries;
using Serilog;

namespace Modmail.NET.Features.Metric.Jobs;

public class StatisticsCalculatorJob : HangfireRecurringJobBase
{
  private readonly IServiceScopeFactory _scopeFactory;

  public StatisticsCalculatorJob(IServiceScopeFactory scopeFactory) : base("statistics-calculator-job", Cron.Daily()) {
    _scopeFactory = scopeFactory;
  }

  public override async Task Execute() {
    Log.Information("Starting Average Data Calculation...");

    var scope = _scopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var guildOption = await sender.Send(new GetOptionQuery());
    var statDays = guildOption.StatisticsCalculateDays;
    if (statDays is < MetricConstants.StatisticsCalculateDaysMin or > MetricConstants.StatisticsCalculateDaysMax) statDays = MetricConstants.DefaultStatisticsCalculateDays;

    var statDate = UtilDate.GetNow().AddDays(-statDays);

    var statistics = new Statistic {
      AvgTicketsClosedPerDay = await GetAvgTicketsClosedPerDay(dbContext, statDate) ?? 0,
      AvgTicketsOpenedPerDay = await GetAvgTicketsOpenedPerDay(dbContext, statDate) ?? 0,
      AvgResponseTimeSeconds = await GetAvgResponseTimeSeconds(dbContext, statDate) ?? 0,
      AvgTicketClosedSeconds = await GetAvgTicketClosedSeconds(dbContext, statDate) ?? 0,
      FastestClosedTicketSeconds = await GetFastestClosedTicketSeconds(dbContext, statDate) ?? 0,
      SlowestClosedTicketSeconds = await GetSlowestClosedTicketSeconds(dbContext, statDate) ?? 0
    };

    dbContext.Add(statistics);
    var affected = await dbContext.SaveChangesAsync();
    if (affected == 0) throw new DbInternalException();

    Log.Information("Average Data Calculation finished");
  }


  private static async Task<double?> GetAvgResponseTimeSeconds(ModmailDbContext dbContext, DateTime statDate) {
    try {
      return await dbContext.Messages
                            .FilterByRegisterDateStart(statDate)
                            .Select(x => new {
                              x.TicketId,
                              x.RegisterDateUtc,
                              x.SentByMod
                            })
                            .GroupBy(x => x.TicketId)
                            .Select(group => new {
                              TicketId = group.Key,
                              FirstUserMessageTime = group
                                                     .Where(x => !x.SentByMod)
                                                     .OrderBy(x => x.RegisterDateUtc)
                                                     .Select(x => x.RegisterDateUtc)
                                                     .DefaultIfEmpty()
                                                     .FirstOrDefault(),
                              FirstModResponseTime = group
                                                     .Where(x => x.SentByMod)
                                                     .OrderBy(x => x.RegisterDateUtc)
                                                     .Select(x => x.RegisterDateUtc)
                                                     .DefaultIfEmpty()
                                                     .FirstOrDefault()
                            })
                            .Where(x => x.FirstUserMessageTime != default && x.FirstModResponseTime != default)
                            .Select(x => EF.Functions.DateDiffSecond(x.FirstUserMessageTime, x.FirstModResponseTime))
                            .DefaultIfEmpty()
                            .AverageAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetAvgResponseTimeSeconds");
    }

    return null;
  }

  private static async Task<double?> GetAvgTicketsClosedPerDay(ModmailDbContext dbContext, DateTime statDate) {
    try {
      return await dbContext.Tickets
                            .FilterByRegisterDateStart(statDate)
                            .FilterByRegisterDate(statDate, UtilDate.GetNow())
                            .FilterClosed()
                            .GroupBy(x => x.ClosedDateUtc!.Value.Date)
                            .Select(group => group.Count())
                            .DefaultIfEmpty()
                            .AverageAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetAvgTicketsClosedPerDay");
      return null;
    }
  }

  private static async Task<double?> GetAvgTicketsOpenedPerDay(ModmailDbContext dbContext, DateTime statDate) {
    try {
      return await dbContext.Tickets
                            .FilterByRegisterDateStart(statDate)
                            .GroupBy(x => x.RegisterDateUtc.Date)
                            .Select(group => group.Count())
                            .DefaultIfEmpty()
                            .AverageAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetAvgTicketsOpenedPerDay");
      return null;
    }
  }

  private static async Task<double?> GetAvgTicketClosedSeconds(ModmailDbContext dbContext, DateTime statDate) {
    try {
      return await dbContext.Tickets
                            .FilterByRegisterDateStart(statDate)
                            .FilterClosed()
                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!.Value))
                            .DefaultIfEmpty()
                            .AverageAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetAvgTicketClosedSeconds");
      return null;
    }
  }

  private static async Task<double?> GetFastestClosedTicketSeconds(ModmailDbContext dbContext, DateTime statDate) {
    try {
      return await dbContext.Tickets
                            .FilterByRegisterDateStart(statDate)
                            .FilterClosed()
                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!.Value))
                            .DefaultIfEmpty()
                            .MinAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetFastestClosedTicketSeconds");
      return null;
    }
  }

  private static async Task<double?> GetSlowestClosedTicketSeconds(ModmailDbContext dbContext, DateTime statDate) {
    try {
      return await dbContext.Tickets
                            .FilterByRegisterDateStart(statDate)
                            .FilterClosed()
                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!.Value))
                            .DefaultIfEmpty()
                            .MaxAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetSlowestClosedTicketSeconds");
      return null;
    }
  }
}