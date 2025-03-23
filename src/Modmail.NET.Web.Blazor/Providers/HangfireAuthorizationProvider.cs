using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public class HangfireAuthorizationProvider : IDashboardAuthorizationFilter
{
  public bool Authorize(DashboardContext context) {
    var httpContext = context.GetHttpContext();

    if (httpContext.User.Identity?.IsAuthenticated != true) return false;

    var authorizationService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();

    //TODO: Optimize async here
    var authorizationResult = authorizationService.AuthorizeAsync(httpContext.User, null, AuthPolicy.ManageHangfire.Name)
                                                  .ConfigureAwait(false)
                                                  .GetAwaiter()
                                                  .GetResult();

    return authorizationResult.Succeeded;
  }
}