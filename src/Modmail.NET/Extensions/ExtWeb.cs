using System.Security.Claims;

namespace Modmail.NET.Extensions;

public static class ExtWeb
{
  public static ulong GetUserId(this ClaimsPrincipal principal) {
    var nameId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (nameId is null) throw new Exception("UserId does not exist in claims");

    var val = ulong.Parse(nameId);

    if (val == 0) throw new Exception("UserId does not exist in claims");

    return val;
  }
}