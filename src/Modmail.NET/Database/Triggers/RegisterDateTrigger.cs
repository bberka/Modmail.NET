using EntityFrameworkCore.Triggered;
using Modmail.NET.Abstract;
using Modmail.NET.Utils;

namespace Modmail.NET.Database.Triggers;

public class RegisterDateTrigger : IBeforeSaveTrigger<IHasRegisterDate>
{
  public Task BeforeSave(ITriggerContext<IHasRegisterDate> context, CancellationToken cancellationToken) {
    if (context.ChangeType is ChangeType.Added) context.Entity.RegisterDateUtc = UtilDate.GetNow();

    return Task.CompletedTask;
  }
}