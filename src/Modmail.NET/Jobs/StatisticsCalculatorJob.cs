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

    double averageResponseTime = 0;
    double avgTicketsOpenPerDay = 0;
    double avgTicketsClosePerDay = 0;
    double avgTicketResolveTime = 0;
    double fastestClosedTicketTime = 0;
    double slowestClosedTicketTime = 0;
    try {
      averageResponseTime = await dbContext.TicketMessages
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
                                           .DefaultIfEmpty()
                                           .AverageAsync() / 60d;
    }
    catch (InvalidOperationException e) {
      //Seq contains no elements
      Log.Verbose(e, "Failed to calculate average response time");
    }


    try {
      avgTicketsOpenPerDay = await dbContext.Tickets
                                                .GroupBy(x => x.RegisterDateUtc.Date)
                                                .Select(x => new {
                                                  Date = x.Key,
                                                  Count = x.Count()
                                                })
                                                .AverageAsync(x => x.Count);
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate average tickets open per day");
    }


    try {
      avgTicketsClosePerDay = await dbContext.Tickets
                                                 .Where(x => x.ClosedDateUtc.HasValue)
                                                 .GroupBy(x => x.RegisterDateUtc.Date)
                                                 .Select(x => new {
                                                   Date = x.Key,
                                                   Count = x.Count()
                                                 })
                                                 .AverageAsync(x => x.Count);
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate average tickets close per day");
    }


    try {
      avgTicketResolveTime = await dbContext.Tickets
                                        .Where(x => x.ClosedDateUtc.HasValue)
                                        .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!))
                                        .DefaultIfEmpty(0)
                                        .AverageAsync() ?? 0 / 60d;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate avg tickets close time");
    }


    try {
      fastestClosedTicketTime = await dbContext.Tickets
                                                   .Where(x => x.ClosedDateUtc.HasValue)
                                                   .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!))
                                                   .DefaultIfEmpty(0)
                                                   .MinAsync() ?? 0 / 60d;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate fastest closed ticket time");
    }


    try {
      slowestClosedTicketTime = await dbContext.Tickets
                                                   .Where(x => x.ClosedDateUtc.HasValue)
                                                   .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc!))
                                                   .DefaultIfEmpty(0)
                                                   .MaxAsync() ?? 0 / 60d;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate longest closed ticket time");
    }
    
    var statistics = new Statistic {
      AvgResponseTimeMinutes = averageResponseTime,
      AvgTicketsClosedPerDay = avgTicketsClosePerDay,
      AvgTicketsOpenedPerDay = avgTicketsOpenPerDay,
      AvgTicketResolvedMinutes =  avgTicketResolveTime,
      FastestClosedTicketMinutes = fastestClosedTicketTime,
      SlowestClosedTicketMinutes = slowestClosedTicketTime
    };
    
    dbContext.Add(statistics);
    var affected = await dbContext.SaveChangesAsync();
    if (affected == 0) throw new DbInternalException();


    Log.Information("Average Data Calculation finished");
  }
}