using Microsoft.Extensions.Caching.Memory;
using Modmail.NET.Database;
using Ninject;
using Ninject.Modules;

namespace Modmail.NET;

public class MmKernel : NinjectModule
{
  public override void Load() {
    Bind<IMemoryCache>().To<Microsoft.Extensions.Caching.Memory.MemoryCache>().InSingletonScope();
    Bind<ModmailDbContext>().ToSelf().InTransientScope();
  }
}

public static class ServiceLocator
{
  private static IKernel _kernel;

  public static void Initialize(IKernel kernel) {
    _kernel = kernel;
  }

  public static T Get<T>() {
    return _kernel.Get<T>();
  }
}