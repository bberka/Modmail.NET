
using Microsoft.Extensions.DependencyInjection;

namespace Modmail.NET;

public static class ServiceLocator
{
  private static IServiceProvider _serviceProvider;

  public static void Initialize(IServiceProvider serviceProvider) {
    _serviceProvider = serviceProvider;
  }

  public static T Get<T>() where T : notnull {
    if (_serviceProvider is null) throw new InvalidOperationException("Kernel is not initialized");

    return _serviceProvider.GetRequiredService<T>();
  }
}