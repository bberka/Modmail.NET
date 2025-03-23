using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Serilog;

namespace Modmail.NET.Jobs;

public sealed class AverageDataCalculatorJob : HangfireRecurringJobBase
{
  private readonly IServiceScopeFactory _scopeFactory;

  public AverageDataCalculatorJob(IServiceScopeFactory scopeFactory) : base("AverageDataCalculatorJob", Cron.Daily()) {
    _scopeFactory = scopeFactory;
  }

  public override async Task Execute() {
    Log.Information("Starting Average Data Calculation...");

    var scope = _scopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var option = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();

    var updated = false;
    try {
      var responseTimes = await dbContext.TicketMessages
                                         .Select(x => new
                                         {
                                           x.TicketId,
                                           x.RegisterDateUtc,
                                           x.SentByMod
                                         })
                                         .GroupBy(x => x.TicketId)
                                         .Select(group => new
                                         {
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
      var averageResponseTime = responseTimes.Count != 0 ? responseTimes.Average() : 0;
      if (averageResponseTime >= 0) option.AvgResponseTimeMinutes = averageResponseTime / 60d;
      updated = true;
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
      option.AvgTicketsOpenPerDay = avgTicketsOpenPerDay;
      updated = true;
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

      option.AvgTicketsClosePerDay = avgTicketsClosePerDay;
      updated = true;
    }
    catch (InvalidOperationException e) {
      Log.Verbose(e, "Failed to calculate average tickets close per day");
    }

    if (updated) {
      dbContext.Update(option);
      var affected = await dbContext.SaveChangesAsync();
      if (affected == 0) throw new DbInternalException();
    }

    Log.Information("Average Data Calculation finished {Updated}", updated);
  }
}