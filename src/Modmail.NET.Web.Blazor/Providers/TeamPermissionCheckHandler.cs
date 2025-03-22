using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Features.Guild;
using Modmail.NET.Static;
using Modmail.NET.Web.Blazor.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public sealed class TeamPermissionCheckHandler : AuthorizationHandler<TeamPermissionCheckRequirement>
{
  private readonly IServiceScopeFactory _scopeFactory;

  public TeamPermissionCheckHandler(IServiceScopeFactory scopeFactory) {
    _scopeFactory = scopeFactory;
  }

  private static readonly IReadOnlyDictionary<AuthPolicy, TeamPermissionLevel> PolicyMap = new Dictionary<AuthPolicy, TeamPermissionLevel>() {
    { AuthPolicy.OnlySupport, TeamPermissionLevel.Support },
    { AuthPolicy.OnlyModerator, TeamPermissionLevel.Moderator },
    { AuthPolicy.OnlyAdmin, TeamPermissionLevel.Admin },
    { AuthPolicy.OnlyOwner, TeamPermissionLevel.Owner },
  };

  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamPermissionCheckRequirement req) {
    var roleClaim = context.User.FindFirst(ClaimTypes.Role);
    if (roleClaim == null) {
      context.Fail();
      return;
    }

    if (!Enum.TryParse<TeamPermissionLevel>(roleClaim.Value, out var userPermissionLevel)) {
      context.Fail();
      return;
    }

    if (PolicyMap.TryGetValue(req.Policy, out var level)) {
      if (userPermissionLevel == level) {
        context.Succeed(req);
        return;
      }
      context.Fail();
      return;
    }

    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var option = await sender.Send(new GetGuildOptionQuery(false));
    if (req.Policy == AuthPolicy.ManageOptions) {
      if (userPermissionLevel >= option.ManageGuildOptionMinAccessLevel) {
        context.Succeed(req);
        return;
      }
    }    
    else if (req.Policy == AuthPolicy.ManageTickets) {
      if (userPermissionLevel >= option.ManageTicketMinAccessLevel) {
        context.Succeed(req);
        return;
      }
    }   
    else if (req.Policy == AuthPolicy.ManageTicketTypes) {
      if (userPermissionLevel >= option.ManageTicketTypeMinAccessLevel) {
        context.Succeed(req);
        return;
      }
    } 
    else if (req.Policy == AuthPolicy.ManageTeams) {
      if (userPermissionLevel >= option.ManageTeamsMinAccessLevel) {
        context.Succeed(req);
        return;
      }
    } 
    else if (req.Policy == AuthPolicy.ManageBlacklist) {
      if (userPermissionLevel >= option.ManageBlacklistMinAccessLevel) {
        context.Succeed(req);
        return;
      }
    }   
    else if (req.Policy == AuthPolicy.ManageHangfire) {
      if (userPermissionLevel >= option.ManageHangfireMinAccessLevel) {
        context.Succeed(req);
        return;
      }
    }

    context.Fail();
  }
}