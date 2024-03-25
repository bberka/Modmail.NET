using System.Collections.Concurrent;
using DSharpPlus.Entities;
using Timer = System.Threading.Timer;

namespace Modmail.NET.Manager;

public class TicketTypeSelectionTimeoutMgr
{
  private static TicketTypeSelectionTimeoutMgr? _instance;

  private TicketTypeSelectionTimeoutMgr() {
    Messages = new();
    // Repeat every 1 seconds
    Timer = new(TimerElapsed, null, 0, 1000);
  }

  public static TicketTypeSelectionTimeoutMgr This {
    get {
      _instance ??= new();
      return _instance;
    }
  }

  public ConcurrentDictionary<DiscordMessage, DateTime> Messages { get; }
  public Timer Timer { get; }

  public void AddMessage(DiscordMessage message) {
    Messages.TryAdd(message, DateTime.UtcNow);
  }

  public void RemoveMessage(DiscordMessage message) {
    Messages.TryRemove(message, out _);
  }

  public void RemoveMessage(ulong id) {
    var message = Messages.FirstOrDefault(x => x.Key.Id == id);
    if (message.Key != null) {
      Messages.TryRemove(message.Key, out _);
    }
  }

  private void TimerElapsed(object? sender) {
    //remove everything that is older than 3 minutes
    foreach (var message in Messages) {
      if (DateTime.UtcNow - message.Value <= TimeSpan.FromMinutes(3)) continue;
      Messages.TryRemove(message.Key, out _);
      message.Key.ModifyAsync(x => {
        x.ClearComponents();
        x.AddEmbeds(message.Key.Embeds);
      });
    }
  }
}