using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Ticket;
using Serilog;

namespace Modmail.NET.Jobs;

public class TicketTimeoutJob : HangfireRecurringJobBase
{
  private readonly IServiceScopeFactory _scopeFactory;

  public TicketTimeoutJob(IServiceScopeFactory scopeFactory) : base("ticket-timeout-job", Cron.Hourly()) {
    _scopeFactory = scopeFactory;
  }

  public override async Task Execute() {
    var scope = _scopeFactory.CreateScope();

    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var bot = scope.ServiceProvider.GetRequiredService<ModmailBot>();

    var guildOption = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();
    var tickets = await sender.Send(new GetTimedOutTicketListQuery(guildOption.TicketTimeoutHours));
    if (tickets.Count > 0)
      foreach (var ticket in tickets) {
        await sender.Send(new ProcessCloseTicketCommand(ticket.Id, bot.Client.CurrentUser.Id, "Ticket timed out"));
        Log.Information("Ticket {TicketId} has been closed due to timeout", ticket.Id);
      }
  }
}