using EntityFrameworkCore.Triggered;
using Modmail.NET.Abstract;

namespace Modmail.NET.Database.Triggers;

public sealed class UpdateDateTrigger : IBeforeSaveTrigger<IHasUpdateDate>
{
  public Task BeforeSave(ITriggerContext<IHasUpdateDate> context, CancellationToken cancellationToken) {
    if (context.ChangeType is ChangeType.Modified) context.Entity.UpdateDateUtc = DateTime.UtcNow;

    return Task.CompletedTask;
  }
}