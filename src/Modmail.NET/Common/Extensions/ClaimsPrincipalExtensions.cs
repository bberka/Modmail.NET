using System.Security.Claims;

namespace Modmail.NET.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static AuthPolicy[] GetPermissions(this ClaimsPrincipal user) {
		var permissionClaimValue = user.FindFirst(AuthConstants.PermissionsClaimType)?.Value;
		if (string.IsNullOrEmpty(permissionClaimValue)) return [];

		return UtilPermission.ParseFromPermissionsString(permissionClaimValue);
	}
}