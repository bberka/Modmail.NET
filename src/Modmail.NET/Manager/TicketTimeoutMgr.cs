using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Serilog;

namespace Modmail.NET.Manager;

public sealed class TicketTimeoutMgr
{
  private static TicketTimeoutMgr? _instance;

  private readonly Timer _timer;

  private TicketTimeoutMgr() {
    const int ticketTimeoutCheckIntervalSeconds = 60;
    _timer = new Timer(TimerElapsed, null, 0, ticketTimeoutCheckIntervalSeconds * 1000);
    Log.Information("{ServiceName} initialized", nameof(TicketTimeoutMgr));
  }

  public static TicketTimeoutMgr This {
    get {
      _instance ??= new TicketTimeoutMgr();
      return _instance;
    }
  }

  private void TimerElapsed(object? sender) {
    TimerElapsedAsync().GetAwaiter().GetResult();
  }

  private async Task TimerElapsedAsync() {
    try {
      var guildOption = await GuildOption.GetAsync();
      var tickets = await Ticket.GetTimeoutTicketsAsync(guildOption.TicketTimeoutHours);
      if (tickets.Count > 0)
        foreach (var ticket in tickets) {
          await ticket.ProcessCloseTicketAsync(ModmailBot.This.Client.CurrentUser.Id, "Ticket timed out");
          Log.Information("Ticket {TicketId} has been closed due to timeout", ticket.Id);
        }
    }
    catch (BotExceptionBase ex) {
      Log.Error(ex, "Failed to check ticket timeout, bot exception occurred");
    }
    catch (Exception ex) {
      Log.Fatal(ex, "Failed to check ticket timeout");
    }
  }
}