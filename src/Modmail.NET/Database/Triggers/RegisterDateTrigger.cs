using EntityFrameworkCore.Triggered;
using Modmail.NET.Abstract;

namespace Modmail.NET.Database.Triggers;

public class RegisterDateTrigger : IBeforeSaveTrigger<IHasRegisterDate>
{
  public Task BeforeSave(ITriggerContext<IHasRegisterDate> context, CancellationToken cancellationToken) {
    if (context.ChangeType is ChangeType.Added) context.Entity.RegisterDateUtc = DateTime.UtcNow;

    return Task.CompletedTask;
  }
}