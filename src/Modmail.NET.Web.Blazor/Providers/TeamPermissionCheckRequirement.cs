using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Web.Blazor.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public sealed class TeamPermissionCheckRequirement : IAuthorizationRequirement
{
  public AuthPolicy Policy { get; }

  public TeamPermissionCheckRequirement(AuthPolicy policy) {
    Policy = policy;
  }
}