using EntityFrameworkCore.Triggered;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Triggers;

public class UpdateDateTrigger : IBeforeSaveTrigger<IHasUpdateDate>
{
  public Task BeforeSave(ITriggerContext<IHasUpdateDate> context, CancellationToken cancellationToken) {
    if (context.ChangeType is ChangeType.Modified) context.Entity.UpdateDateUtc = UtilDate.GetNow();

    return Task.CompletedTask;
  }
}