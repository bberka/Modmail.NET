using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Models;
using Modmail.NET.Static;

namespace Modmail.NET.Utils;

public static class UtilPermission
{
  public static ( List<DiscordMember> members, List<DiscordRole> roles) ParsePermissionInfo(List<PermissionInfo> permissions,
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
    allPerm.Deny(Permissions.All);
    overwrites.Add(allPerm);

    foreach (var member in members) {
      var memberPerm = new DiscordOverwriteBuilder(member);
      memberPerm.Allow(Permissions.AccessChannels);
      memberPerm.Allow(Permissions.ReadMessageHistory);
      memberPerm.Allow(Permissions.SendMessages);
      memberPerm.Allow(Permissions.EmbedLinks);
      memberPerm.Allow(Permissions.AttachFiles);
      memberPerm.Deny(Permissions.ManageChannels);

      overwrites.Add(memberPerm);
    }

    foreach (var role in roles) {
      var rolePerm = new DiscordOverwriteBuilder(role);
      rolePerm.Allow(Permissions.AccessChannels);
      rolePerm.Allow(Permissions.ReadMessageHistory);
      rolePerm.Allow(Permissions.SendMessages);
      rolePerm.Allow(Permissions.EmbedLinks);
      rolePerm.Allow(Permissions.AttachFiles);
      rolePerm.Deny(Permissions.ManageChannels);

      overwrites.Add(rolePerm);
    }

    return overwrites;
  }
}