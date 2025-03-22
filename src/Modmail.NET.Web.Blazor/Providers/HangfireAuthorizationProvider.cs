using Hangfire.Dashboard;
using Modmail.NET.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public class HangfireAuthorizationProvider : IDashboardAuthorizationFilter
{
  private readonly IServiceProvider _appServices;

  public HangfireAuthorizationProvider(IServiceProvider appServices) {
    _appServices = appServices;
  }
  public bool Authorize(DashboardContext context) {
    var httpContext = context.GetHttpContext();

    if (httpContext.User.Identity?.IsAuthenticated == true) {
      return false;
    }

    //TODO: Db option auth
    // var scope = _appServices.CreateScope();
    // var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var allowed = httpContext.User.IsInRole(TeamPermissionLevel.Admin.ToString()) || httpContext.User.IsInRole(TeamPermissionLevel.Owner.ToString());
    return allowed;
  }
}