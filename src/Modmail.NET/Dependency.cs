using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;
using Ninject;
using Ninject.Modules;

namespace Modmail.NET;

public class MmKernel : NinjectModule
{
  public override void Load() {
    Bind<IOptions<MemoryCacheOptions>>().To<BotCacheOptions>().InSingletonScope();
    Bind<IMemoryCache>().To<MemoryCache>().InSingletonScope();
    Bind<ModmailDbContext>().ToSelf().InTransientScope();
  }
}

public static class ServiceLocator
{
  private static IKernel? _kernel;

  public static void Initialize(IKernel kernel) {
    _kernel = kernel;
  }

  public static T Get<T>() {
    if (_kernel is null) throw new InvalidOperationException("Kernel is not initialized");

    return _kernel.Get<T>();
  }
}

public class BotCacheOptions : MemoryCacheOptions
{
  public BotCacheOptions() {
    ExpirationScanFrequency = TimeSpan.FromSeconds(1);
  }
}