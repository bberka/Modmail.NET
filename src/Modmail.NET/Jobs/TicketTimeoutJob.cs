using Hangfire;
using Modmail.NET.Entities;
using Serilog;

namespace Modmail.NET.Jobs;

public sealed class TicketTimeoutJob : HangfireRecurringJobBase
{
  public TicketTimeoutJob() : base("TicketTimeoutJob", Cron.Hourly()) { }

  public override async Task Execute() {
    var guildOption = await GuildOption.GetAsync();
    var tickets = await Ticket.GetTimeoutTicketsAsync(guildOption.TicketTimeoutHours);
    if (tickets.Count > 0)
      foreach (var ticket in tickets) {
        await ticket.ProcessCloseTicketAsync(ModmailBot.This.Client.CurrentUser.Id, "Ticket timed out");
        Log.Information("Ticket {TicketId} has been closed due to timeout", ticket.Id);
      }
  }
}