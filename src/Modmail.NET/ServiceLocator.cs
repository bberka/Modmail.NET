using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Language;

namespace Modmail.NET;

/// <summary>
///   Public static service provider accessor.
///   Since this class it self is an anti pattern we are only allowing certain classes to be instantiated.
///   We are mostly allowing singleton classes to be accessed through here.
///   This method is only for static classes and extension methods, where instead of passing the initialized classes down
///   the line to underlying methods.
///   Underlying methods can easily access these singleton classes.
///   Think twice before adding a method here.
///   Never ever add a generic Get or CreateService method.
/// </summary>
public static class ServiceLocator
{
	private static IServiceProvider? _serviceProvider;

	public static void Initialize(IServiceProvider serviceProvider) {
		if (_serviceProvider is not null) throw new InvalidOperationException("ServiceLocator.ServiceProvider has already been initialized.");
		_serviceProvider = serviceProvider;
	}

	public static ModmailBot GetModmailBot() {
		return _serviceProvider?.GetRequiredService<ModmailBot>() ?? throw new InvalidOperationException("ServiceLocator.ServiceProvider has not been initialized.");
	}

	public static BotConfig GetBotConfig() {
		return _serviceProvider?.GetRequiredService<IOptions<BotConfig>>().Value ?? throw new InvalidOperationException("ServiceLocator.ServiceProvider has not been initialized.");
	}

	public static LangProvider GetLangProvider() {
		return _serviceProvider?.GetRequiredService<LangProvider>() ?? throw new InvalidOperationException("ServiceLocator.ServiceProvider has not been initialized.");
	}

	public static ISender CreateSender() {
		var scope = _serviceProvider?.CreateScope();
		return scope?.ServiceProvider.GetRequiredService<ISender>() ?? throw new InvalidOperationException("ServiceLocator.ServiceProvider has not been initialized.");
	}

	public static HttpClient CreateHttpClient() {
		return _serviceProvider?.GetRequiredService<HttpClient>() ?? throw new InvalidOperationException("ServiceLocator.ServiceProvider has not been initialized.");
	}
}