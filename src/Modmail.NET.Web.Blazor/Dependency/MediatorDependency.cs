using Modmail.NET.Pipeline;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class MediatorDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddMediatR(x => {
      x.RegisterServicesFromAssemblies(typeof(ModmailBot).Assembly);
      x.AddOpenBehavior(typeof(LoggerPipelineBehavior<,>));
      x.AddOpenBehavior(typeof(ValidationBehavior<,>));
      x.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
      x.AddOpenBehavior(typeof(RetryPipelineBehavior<,>));
      x.AddOpenBehavior(typeof(PermissionCheckPipelineBehavior<,>));
      x.Lifetime = ServiceLifetime.Scoped;
    });
  }
}