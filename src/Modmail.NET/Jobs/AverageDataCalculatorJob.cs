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

    const string sql = """
                       SELECT
                        DATEDIFF(SECOND, ticket.RegisterDateUtc, adminMessage.FirstAdminMessageRegisterDateUtc) AS Value
                       FROM
                        Tickets ticket
                        OUTER APPLY (
                        SELECT TOP 1 RegisterDateUtc AS FirstAdminMessageRegisterDateUtc
                        FROM TicketMessages
                        WHERE
                        TicketId = ticket.Id
                        AND SenderUserId != ticket.OpenerUserId
                        ORDER BY
                        RegisterDateUtc ASC
                        ) AS adminMessage
                       """;

    var averageResponseTime = await dbContext.Database.SqlQueryRaw<int>(sql).FirstOrDefaultAsync();
    var option = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();

    if (averageResponseTime >= 0) option.AvgResponseTimeMinutes = averageResponseTime / 60d;

    var avgTicketsOpenPerDay = await dbContext.Tickets
                                              .GroupBy(x => x.RegisterDateUtc.Date)
                                              .Select(x => new {
                                                Date = x.Key,
                                                Count = x.Count()
                                              })
                                              .AverageAsync(x => x.Count);
    option.AvgTicketsOpenPerDay = avgTicketsOpenPerDay;


    var avgTicketsClosePerDay = await dbContext.Tickets
                                               .Where(x => !x.ClosedDateUtc.HasValue)
                                               .GroupBy(x => x.RegisterDateUtc.Date)
                                               .Select(x => new {
                                                 Date = x.Key,
                                                 Count = x.Count()
                                               })
                                               .AverageAsync(x => x.Count);

    option.AvgTicketsClosePerDay = avgTicketsClosePerDay;


    dbContext.Update(option);
    var affected = await dbContext.SaveChangesAsync();
    if (affected == 0) throw new DbInternalException();

    Log.Information("Average Data Calculation finished");
  }
}