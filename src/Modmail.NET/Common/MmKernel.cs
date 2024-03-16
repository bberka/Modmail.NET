using Modmail.NET.Abstract.Services;
using Modmail.NET.Database;
using Modmail.NET.Database.Services;
using Ninject.Modules;

namespace Modmail.NET.Common;

public class MmKernel : NinjectModule
{
  public override void Load() {
    Bind<IDbService>().To<DbService>().InTransientScope();
    Bind<ModmailDbContext>().ToSelf().InTransientScope();
  }
}