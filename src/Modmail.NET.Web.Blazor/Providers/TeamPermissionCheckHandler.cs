using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Web.Blazor.Extensions;

namespace Modmail.NET.Web.Blazor.Providers;

public class TeamPermissionCheckHandler : AuthorizationHandler<TeamPermissionCheckRequirement>
{
  private readonly IServiceScopeFactory _scopeFactory;

  public TeamPermissionCheckHandler(IServiceScopeFactory scopeFactory) {
    _scopeFactory = scopeFactory;
  }

  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamPermissionCheckRequirement req) {
    var userId = context.User.GetUserId();
    using var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var canAccess = await sender.Send(new CheckPermissionAccessQuery(userId, req.Policy));
    if (canAccess) {
      context.Succeed(req);
      return;
    }

    context.Fail();
  }
}