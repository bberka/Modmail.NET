using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Features.Permission.Static;
using Modmail.NET.Web.Blazor.Extensions;

namespace Modmail.NET.Web.Blazor.Providers;

public class TeamPermissionCheckHandler : AuthorizationHandler<TeamPermissionCheckRequirement>
{
  private readonly IServiceScopeFactory _scopeFactory;

  public TeamPermissionCheckHandler(IServiceScopeFactory scopeFactory) {
    _scopeFactory = scopeFactory;
  }

  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamPermissionCheckRequirement req) {
    var roleClaim = context.User.FindFirst(ClaimTypes.Role);
    if (roleClaim == null) {
      context.Fail();
      return;
    }

    if (!Enum.TryParse<TeamPermissionLevel>(roleClaim.Value, out _)) {
      context.Fail();
      return;
    }

    var userId = context.User.GetUserId();
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var allowed = await sender.Send(new CheckPermissionAccessQuery(userId, req.Policy));
    if (!allowed) {
      context.Fail();
      return;
    }

    context.Succeed(req);
  }
}