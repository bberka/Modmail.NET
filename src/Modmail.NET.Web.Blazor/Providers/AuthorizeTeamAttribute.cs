using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Static;
using Modmail.NET.Web.Blazor.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public sealed class AuthorizeTeamAttribute : AuthorizeAttribute
{
  public AuthorizeTeamAttribute() {
    Roles = string.Join(",", Enum.GetNames<TeamPermissionLevel>());
  }

  public AuthorizeTeamAttribute(AuthPolicy policy) {
    Policy = policy.Name;
  }
  
  public AuthorizeTeamAttribute(string policy) {
    Policy = policy;
  }
}