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

		var role = principal.GetRole();
		if (role != AuthConstants.AuthorizeTeamRole) throw new ModmailBotException(Lang.UnauthorizedAccess);

		return val;
	}

	public static string GetUsername(this ClaimsPrincipal principal) {
		var name = principal.FindFirst(ClaimTypes.Name)?.Value;
		if (name is null) throw new Exception("Username does not exist in claims");

		return name;
	}

	public static string GetRole(this ClaimsPrincipal principal) {
		var role = principal.FindFirst(ClaimTypes.Role)?.Value;
		if (role is null) throw new Exception("Role does not exist in claims");

		return role;
	}
}