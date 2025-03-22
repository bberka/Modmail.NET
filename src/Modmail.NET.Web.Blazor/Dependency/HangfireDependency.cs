using Hangfire;
using Modmail.NET.Abstract;
using Modmail.NET.Web.Blazor.Providers;
using Serilog;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class HangfireDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddHangfire(configuration => configuration
                                                  .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                                  .UseSimpleAssemblyNameTypeSerializer()
                                                  .UseRecommendedSerializerSettings()
                                                  .UseSqlServerStorage(builder.Configuration.GetValue<string>("Bot:DbConnectionString")));
    builder.Services.Scan(scan => scan
                                  .FromAssemblyOf<IRecurringJobDefinition>()
                                  .AddClasses(classes => classes.AssignableTo<IRecurringJobDefinition>())
                                  .AsSelf() 
                                  .AsImplementedInterfaces() 
                                  .WithSingletonLifetime());

    builder.Services.AddHangfireServer();
  }

  public static void Initialize(WebApplication app) {
    app.UseHangfireDashboard("/hangfire", new DashboardOptions() {
      Authorization = [
        new HangfireAuthorizationProvider()
      ]
    });
    
    using var scope = app.Services.CreateScope();
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var registeredJobDefinitions = scope.ServiceProvider.GetServices<IRecurringJobDefinition>().ToArray();
    if (registeredJobDefinitions.Length == 0) {
      Log.Error("No registered job definitions found");
    }
    foreach (var jobDefinition in registeredJobDefinitions) jobDefinition.RegisterRecurringJob(recurringJobManager);
  }
}