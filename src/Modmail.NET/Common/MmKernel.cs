using Modmail.NET.Database;
using Ninject.Modules;

namespace Modmail.NET.Common;

public class MmKernel : NinjectModule
{
  public override void Load() {
    // Bind<IDbService>().To<DbService>().InTransientScope();
    Bind<ModmailDbContext>().ToSelf().InTransientScope();
  }
}