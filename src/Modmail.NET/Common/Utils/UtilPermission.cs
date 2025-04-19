using DSharpPlus.Entities;
using Modmail.NET.Features.Teams.Models;

namespace Modmail.NET.Common.Utils;

public static class UtilPermission
{
	public static DiscordMember[] ParsePermissionInfo(UserTeamInformation[] userTeamInformations,
	                                                  IReadOnlyCollection<DiscordMember> members) {
		var modMemberListForOverwrites = new List<DiscordMember>();
		foreach (var perm in userTeamInformations) {
			var member = members.FirstOrDefault(x => x.Id == perm.UserId);
			if (member is null || member.Id == 0) continue;
			var exists = modMemberListForOverwrites.Any(x => x.Id == member.Id);
			if (!exists)
				modMemberListForOverwrites.Add(member);
		}

		return modMemberListForOverwrites.ToArray();
	}

	public static DiscordOverwriteBuilder[] GetTicketPermissionOverwrites(DiscordGuild guild,
	                                                                      DiscordMember[] members) {
		var overwrites = new List<DiscordOverwriteBuilder>();

		var allPerm = new DiscordOverwriteBuilder(guild.EveryoneRole);
		allPerm.Deny(DiscordPermissions.All);
		allPerm.Allow(new DiscordPermissions(DiscordPermission.MentionEveryone));
		allPerm.Allow(new DiscordPermissions(DiscordPermission.AttachFiles));
		allPerm.Allow(new DiscordPermissions(DiscordPermission.EmbedLinks));
		overwrites.Add(allPerm);

		foreach (var member in members) {
			var memberPerm = new DiscordOverwriteBuilder(member);
			memberPerm.Allow(new DiscordPermissions(DiscordPermission.ViewChannel));
			memberPerm.Allow(new DiscordPermissions(DiscordPermission.ReadMessageHistory));
			memberPerm.Allow(new DiscordPermissions(DiscordPermission.SendMessages));
			memberPerm.Deny(new DiscordPermissions(DiscordPermission.ManageChannels));

			overwrites.Add(memberPerm);
		}

		return overwrites.ToArray();
	}

	public static AuthPolicy[] ParseFromPermissionsString(string permissionsString) {
		if (!permissionsString.Contains(',')) {
			if (AuthPolicy.TryFromName(permissionsString, true, out var result)) return [result];

			return [];
		}

		var splitedClaimValue = permissionsString.Split(',');
		var list = new List<AuthPolicy>();
		foreach (var claimValue in splitedClaimValue)
			if (AuthPolicy.TryFromName(claimValue, true, out var result))
				list.Add(result);

		return list.ToArray();
	}

	public static string ParseToPermissionsString(AuthPolicy[] permissions) {
		return string.Join(",", permissions.Select(x => x.Name.ToString()));
	}
}