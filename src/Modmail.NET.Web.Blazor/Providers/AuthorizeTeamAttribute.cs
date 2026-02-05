using Microsoft.AspNetCore.Authorization;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Web.Blazor.Providers;

public class AuthorizeTeamAttribute : AuthorizeAttribute
{
    public AuthorizeTeamAttribute()
    {
        Roles = AuthConstants.AuthorizeTeamRole;
    }

    public AuthorizeTeamAttribute(AuthPolicy policy)
    {
        Roles = AuthConstants.AuthorizeTeamRole;
        Policy = policy.Name;
    }

    public AuthorizeTeamAttribute(string policy)
    {
        Roles = AuthConstants.AuthorizeTeamRole;
        Policy = policy;
    }
}