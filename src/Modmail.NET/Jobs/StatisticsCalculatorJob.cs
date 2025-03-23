using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Serilog;

namespace Modmail.NET.Jobs;

public class StatisticsCalculatorJob : HangfireRecurringJobBase
{
  private readonly IServiceScopeFactory _scopeFactory;

  public StatisticsCalculatorJob(IServiceScopeFactory scopeFactory) : base("StatisticsCalculatorJob", Cron.Daily()) {
    _scopeFactory = scopeFactory;
  }

  public override async Task Execute() {
    Log.Information("Starting Average Data Calculation...");

    var scope = _scopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

    var statistics = new Statistic();
    try {
      //TODO: Improve performance of this db call
      var responseTimes = await dbContext.TicketMessages
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
                                                                  .FirstOrDefault(),
                                           FirstModResponseTime = group
                                                                  .Where(x => x.SentByMod)
                                                                  .OrderBy(x => x.RegisterDateUtc)
                                                                  .Select(x => x.RegisterDateUtc)
                                                                  .FirstOrDefault()
                                         })
                                         .Where(x => x.FirstUserMessageTime != default && x.FirstModResponseTime != default)
                                         .Select(x => EF.Functions.DateDiffSecond(x.FirstUserMessageTime, x.FirstModResponseTime))
                                         .ToListAsync();
      var averageResponseTime = responseTimes.Count != 0
                                  ? responseTimes.Average()
                                  : 0;
      if (averageResponseTime >= 0) statistics.AvgResponseTimeMinutes = averageResponseTime / 60d;
    }
    catch (InvalidOperationException e) {
      //Seq contains no elements
      Log.Verbose(e, "Failed to calculate average response time");
    }


    try {
      var avgTicketsOpenPerDay = await dbContext.Tickets
                                                .GroupBy(x => x.RegisterDateUtc.Date)
                                                .Select(x => new {
                                                  Date = x.Key,
                                                  Count = x.Count()
                                                })
                                                .AverageAsync(x => x.Count);
      statistics.AvgTicketsOpenedPerDay = avgTicketsOpenPerDay;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate average tickets open per day");
    }


    try {
      var avgTicketsClosePerDay = await dbContext.Tickets
                                                 .Where(x => x.ClosedDateUtc.HasValue)
                                                 .GroupBy(x => x.RegisterDateUtc.Date)
                                                 .Select(x => new {
                                                   Date = x.Key,
                                                   Count = x.Count()
                                                 })
                                                 .AverageAsync(x => x.Count);

      statistics.AvgTicketsClosedPerDay = avgTicketsClosePerDay;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate average tickets close per day");
    }


    try {
      var avgTicketResolve = await dbContext.Tickets
                                            .Where(x => x.ClosedDateUtc.HasValue)
                                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!))
                                            .AverageAsync();

      if (avgTicketResolve.HasValue) statistics.AvgTicketResolvedMinutes = avgTicketResolve.Value / 60d;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate avg tickets close time");
    }


    try {
      var fastestClosedTicketTime = await dbContext.Tickets
                                                   .Where(x => x.ClosedDateUtc.HasValue)
                                                   .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!))
                                                   .MinAsync();

      if (fastestClosedTicketTime.HasValue) statistics.FastestClosedTicketMinutes = fastestClosedTicketTime.Value / 60d;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate fastest closed ticket time");
    }


    try {
      var slowestClosedTicketTime = await dbContext.Tickets
                                                   .Where(x => x.ClosedDateUtc.HasValue)
                                                   .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!))
                                                   .MaxAsync();

      if (slowestClosedTicketTime.HasValue) statistics.SlowestClosedTicketMinutes = slowestClosedTicketTime.Value / 60d;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate longest closed ticket time");
    }

    dbContext.Add(statistics);
    var affected = await dbContext.SaveChangesAsync();
    if (affected == 0) throw new DbInternalException();


    Log.Information("Average Data Calculation finished");
  }
}