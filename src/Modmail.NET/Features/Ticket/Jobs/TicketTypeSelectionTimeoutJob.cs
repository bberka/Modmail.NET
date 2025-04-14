using System.Collections.Concurrent;
using DSharpPlus.Entities;
using Hangfire;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Utils;

namespace Modmail.NET.Features.Ticket.Jobs;

public class TicketTypeSelectionTimeoutJob : HangfireRecurringJobBase
{
  public TicketTypeSelectionTimeoutJob() : base("ticket-type-selection-timeout-job", Cron.Minutely()) {
    Messages = new ConcurrentDictionary<DiscordMessage, DateTime>();
  }

  public ConcurrentDictionary<DiscordMessage, DateTime> Messages { get; private set; }

  public void AddMessage(DiscordMessage message) {
    Messages.TryAdd(message, UtilDate.GetNow());
  }

  public void RemoveMessage(DiscordMessage message) {
    Messages.TryRemove(message, out _);
  }

  public void RemoveMessage(ulong id) {
    var message = Messages.FirstOrDefault(x => x.Key.Id == id);
    if (message.Key is not null) Messages.TryRemove(message.Key, out _);
  }


  public override async Task Execute() {
    var newDict = new ConcurrentDictionary<DiscordMessage, DateTime>();
    foreach (var message in Messages) {
      if (UtilDate.GetNow() - message.Value <= TimeSpan.FromMinutes(3)) {
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