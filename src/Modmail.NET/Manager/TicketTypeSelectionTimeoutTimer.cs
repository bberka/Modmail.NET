using System.Collections.Concurrent;
using DSharpPlus.Entities;
using Serilog;
using Timer = System.Threading.Timer;

namespace Modmail.NET.Manager;

public sealed class TicketTypeSelectionTimeoutTimer
{
  private static TicketTypeSelectionTimeoutTimer? _instance;

  private TicketTypeSelectionTimeoutTimer() {
    Messages = new ConcurrentDictionary<DiscordMessage, DateTime>();
    // Repeat every 1 seconds
    Timer = new Timer(TimerElapsed, null, 0, 1000);
    Log.Information("{ServiceName} initialized", nameof(TicketTypeSelectionTimeoutTimer));
  }

  public static TicketTypeSelectionTimeoutTimer This {
    get {
      _instance ??= new TicketTypeSelectionTimeoutTimer();
      return _instance;
    }
  }

  public ConcurrentDictionary<DiscordMessage, DateTime> Messages { get; private set; }
  public Timer Timer { get; }

  public void AddMessage(DiscordMessage message) {
    Messages.TryAdd(message, DateTime.UtcNow);
  }

  public void RemoveMessage(DiscordMessage message) {
    Messages.TryRemove(message, out _);
  }

  public void RemoveMessage(ulong id) {
    var message = Messages.FirstOrDefault(x => x.Key.Id == id);
    if (message.Key != null) Messages.TryRemove(message.Key, out _);
  }

  private void TimerElapsed(object? sender) {
    //remove everything that is older than 3 minutes

    var newDict = new ConcurrentDictionary<DiscordMessage, DateTime>();
    foreach (var message in Messages) {
      if (DateTime.UtcNow - message.Value <= TimeSpan.FromMinutes(3)) {
        newDict.TryAdd(message.Key, message.Value);
        continue;
      }

      message.Key.ModifyAsync(x => {
        x.ClearComponents();
        x.AddEmbeds(message.Key.Embeds);
      });
    }

    Messages = newDict;
  }
}