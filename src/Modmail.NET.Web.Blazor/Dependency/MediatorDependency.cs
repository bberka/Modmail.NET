using Modmail.NET.Pipeline;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class MediatorDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddMediatR(x => {
      x.RegisterServicesFromAssemblies(typeof(ModmailBotProjectMarker).Assembly);
      x.AddOpenBehavior(typeof(LoggerPipelineBehavior<,>));
      x.AddOpenBehavior(typeof(ValidationBehavior<,>));
      x.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
      x.AddOpenBehavior(typeof(RetryPipelineBehavior<,>));
      x.Lifetime = ServiceLifetime.Scoped;
    });
  }
}