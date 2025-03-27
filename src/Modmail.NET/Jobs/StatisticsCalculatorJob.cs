using System.Linq.Dynamic.Core;
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

    var statistics = new Statistic {
      AvgTicketsClosedPerDay = await GetAvgTicketsClosedPerDay(dbContext) ?? 0,
      AvgTicketsOpenedPerDay = await GetAvgTicketsOpenedPerDay(dbContext) ?? 0,
      AvgResponseTimeSeconds = await GetAvgResponseTimeSeconds(dbContext) ?? 0,
      AvgTicketClosedSeconds = await GetAvgTicketClosedSeconds(dbContext) ?? 0,
      FastestClosedTicketSeconds = await GetFastestClosedTicketSeconds(dbContext) ?? 0,
      SlowestClosedTicketSeconds = await GetSlowestClosedTicketSeconds(dbContext) ?? 0,
    };

    dbContext.Add(statistics);
    var affected = await dbContext.SaveChangesAsync();
    if (affected == 0) throw new DbInternalException();

    Log.Information("Average Data Calculation finished");
  }


  private static async Task<double?> GetAvgResponseTimeSeconds(ModmailDbContext dbContext) {
    try {
      return await dbContext.TicketMessages
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

  private static async Task<double?> GetAvgTicketsClosedPerDay(ModmailDbContext dbContext) {
    try {
      return await dbContext.Tickets
                            .Where(x => x.ClosedDateUtc.HasValue)
                            .GroupBy(x => x.ClosedDateUtc.Value.Date)
                            .Select(group => group.Count())
                            .DefaultIfEmpty()
                            .AverageAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetAvgTicketsClosedPerDay");
      return null;
    }
  }

  private static async Task<double?> GetAvgTicketsOpenedPerDay(ModmailDbContext dbContext) {
    try {
      return await dbContext.Tickets
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

  private static async Task<double?> GetAvgTicketClosedSeconds(ModmailDbContext dbContext) {
    try {
      return await dbContext.Tickets
                            .Where(x => x.ClosedDateUtc.HasValue)
                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc.Value))
                            .DefaultIfEmpty()
                            .AverageAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetAvgTicketClosedSeconds");
      return null;
    }
  }

  private static async Task<double?> GetFastestClosedTicketSeconds(ModmailDbContext dbContext) {
    try {
      return await dbContext.Tickets
                            .Where(x => x.ClosedDateUtc.HasValue)
                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc.Value))
                            .DefaultIfEmpty()
                            .MinAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetFastestClosedTicketSeconds");
      return null;
    }
  }

  private static async Task<double?> GetSlowestClosedTicketSeconds(ModmailDbContext dbContext) {
    try {
      return await dbContext.Tickets
                            .Where(x => x.ClosedDateUtc.HasValue)
                            .Select(x => EF.Functions.DateDiffSecond(x.RegisterDateUtc, x.ClosedDateUtc.Value))
                            .DefaultIfEmpty()
                            .MaxAsync();
    }
    catch (Exception e) {
      Log.Error(e, "Failed to calculate GetSlowestClosedTicketSeconds");
      return null;
    }
  }
}