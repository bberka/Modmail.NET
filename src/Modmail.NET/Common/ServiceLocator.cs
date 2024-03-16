using Ninject;

namespace Modmail.NET.Common;

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