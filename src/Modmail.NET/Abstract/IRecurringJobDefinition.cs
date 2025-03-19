using Hangfire;

namespace Modmail.NET.Abstract;

public interface IRecurringJobDefinition
{
  void RegisterRecurringJob(IRecurringJobManager recurringJobManager);
}
