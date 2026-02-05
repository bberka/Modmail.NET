using EntityFrameworkCore.Triggered;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Triggers;

public class IdentityV7Trigger : IBeforeSaveTrigger<IGuidId>
{
  public Task BeforeSave(ITriggerContext<IGuidId> context, CancellationToken cancellationToken) {
    if (context.ChangeType is ChangeType.Added)
      if (context.Entity.Id == Guid.Empty)
        context.Entity.Id = Guid.CreateVersion7();

    return Task.CompletedTask;
  }
}