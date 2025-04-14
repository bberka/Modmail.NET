using System.Security.Claims;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Web.Blazor.Extensions;

public static class WebExtensions
{
  public static ulong GetUserId(this ClaimsPrincipal principal) {
    var nameId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (nameId is null) throw new Exception("UserId does not exist in claims");

    var val = ulong.Parse(nameId);

    if (val == 0) throw new Exception("UserId does not exist in claims");

    var role = principal.FindFirst(ClaimTypes.Role)?.Value;
    if (role != AuthConstants.AuthorizeTeamRole) throw new ModmailBotException(Lang.UnauthorizedAccess);

    return val;
  }
}