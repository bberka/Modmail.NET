using EntityFrameworkCore.Triggered;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Triggers;

public class RegisterDateTrigger : IBeforeSaveTrigger<IRegisterDateUtc>
{
  public Task BeforeSave(ITriggerContext<IRegisterDateUtc> context, CancellationToken cancellationToken) {
    if (context.ChangeType is ChangeType.Added) context.Entity.RegisterDateUtc = UtilDate.GetNow();

    return Task.CompletedTask;
  }
}