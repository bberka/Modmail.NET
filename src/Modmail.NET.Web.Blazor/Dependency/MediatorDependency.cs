using Modmail.NET.Pipeline;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class MediatorDependency
{
	public static void Configure(WebApplicationBuilder builder) {
		MediatorDependencyInjectionExtensions.AddMediator(builder.Services, x => { x.ServiceLifetime = ServiceLifetime.Transient; });
		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerPipelineBehavior<,>));
		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>));
		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryPipelineBehavior<,>));
		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PermissionCheckPipelineBehavior<,>));
	}
}