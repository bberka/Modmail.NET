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
		Log.Information("[RECURRING JOB] Registering {Id}", Id);
		recurringJobManager.AddOrUpdate(
		                                Id,
		                                () => Execute(),
		                                CronExpression
		                               );
	}

	public abstract Task Execute();

	public void TriggerJob() {
		RecurringJob.TriggerJob(Id);
	}
}