using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public class TeamPermissionCheckRequirement : IAuthorizationRequirement
{
  public TeamPermissionCheckRequirement(AuthPolicy policy) {
    Policy = policy;
  }

  public AuthPolicy Policy { get; }
}