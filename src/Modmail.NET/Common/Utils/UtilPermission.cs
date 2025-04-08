using DSharpPlus.Entities;
using Modmail.NET.Features.Permission.Models;
using Modmail.NET.Features.Teams.Static;

namespace Modmail.NET.Common.Utils;

public static class UtilPermission
{
  public static ( List<DiscordMember> members, List<DiscordRole> roles) ParsePermissionInfo(PermissionInfo[] permissions,
                                                                                            IReadOnlyCollection<DiscordMember> members,
                                                                                            IReadOnlyDictionary<ulong, DiscordRole> roles) {
    var modRoleListForOverwrites = new List<DiscordRole>();
    var modMemberListForOverwrites = new List<DiscordMember>();
    foreach (var perm in permissions) {
      var role = roles.FirstOrDefault(x => x.Key == perm.Key && perm.Type == TeamMemberDataType.RoleId);
      if (role.Key != 0) {
        var exists = modRoleListForOverwrites.Any(x => x.Id == role.Key);
        if (!exists)
          modRoleListForOverwrites.Add(role.Value);
      }

      var member2 = members.FirstOrDefault(x => x.Id == perm.Key && perm.Type == TeamMemberDataType.UserId);
      if (member2 is not null && member2.Id != 0) {
        var exists = modMemberListForOverwrites.Any(x => x.Id == member2.Id);
        if (!exists)
          modMemberListForOverwrites.Add(member2);
      }
    }

    return (modMemberListForOverwrites, modRoleListForOverwrites);
  }

  public static List<DiscordOverwriteBuilder> GetTicketPermissionOverwrites(DiscordGuild guild,
                                                                            List<DiscordMember> members,
                                                                            List<DiscordRole> roles) {
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

    foreach (var role in roles) {
      var rolePerm = new DiscordOverwriteBuilder(role);
      rolePerm.Allow(new DiscordPermissions(DiscordPermission.ViewChannel));
      rolePerm.Allow(new DiscordPermissions(DiscordPermission.ReadMessageHistory));
      rolePerm.Allow(new DiscordPermissions(DiscordPermission.SendMessages));
      rolePerm.Deny(new DiscordPermissions(DiscordPermission.ManageChannels));

      overwrites.Add(rolePerm);
    }

    return overwrites;
  }
}