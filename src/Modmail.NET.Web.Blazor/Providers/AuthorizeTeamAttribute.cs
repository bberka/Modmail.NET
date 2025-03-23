using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public class AuthorizeTeamAttribute : AuthorizeAttribute
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