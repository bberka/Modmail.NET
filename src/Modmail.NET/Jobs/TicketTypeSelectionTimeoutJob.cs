using System.Collections.Concurrent;
using DSharpPlus.Entities;
using Hangfire;
using Modmail.NET.Abstract;

namespace Modmail.NET.Jobs;

public sealed class TicketTypeSelectionTimeoutJob : HangfireRecurringJobBase
{
  public TicketTypeSelectionTimeoutJob() : base("TicketTypeSelectionTimeoutJob", Cron.Minutely()) {
    Messages = new ConcurrentDictionary<DiscordMessage, DateTime>();
  }

  public ConcurrentDictionary<DiscordMessage, DateTime> Messages { get; private set; }

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


  public override async Task Execute() {
    var newDict = new ConcurrentDictionary<DiscordMessage, DateTime>();
    foreach (var message in Messages) {
      if (DateTime.UtcNow - message.Value <= TimeSpan.FromMinutes(3)) {
        newDict.TryAdd(message.Key, message.Value);
        continue;
      }

      await message.Key.ModifyAsync(x => {
        x.ClearComponents();
        x.AddEmbeds(message.Key.Embeds);
      });
    }

    Messages = newDict;
  }
}