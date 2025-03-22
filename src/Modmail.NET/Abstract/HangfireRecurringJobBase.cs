using Hangfire;
using Serilog;

namespace Modmail.NET.Abstract;

public abstract class HangfireRecurringJobBase : IRecurringJobDefinition
{
  protected HangfireRecurringJobBase(string id, string cronExpression) {
    Id = id;
    CronExpression = cronExpression;
  }

  protected string CronExpression { get; }
  protected string Id { get; }

  public virtual void RegisterRecurringJob(IRecurringJobManager recurringJobManager) {
    Log.Information("Registering Recurring Job {Id}", Id);
    recurringJobManager.AddOrUpdate(
                                    Id.ToLowerInvariant(),
                                    () => Execute(),
                                    CronExpression
                                   );
  }

  public abstract Task Execute();
}