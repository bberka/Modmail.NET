using DSharpPlus;
using DSharpPlus.Entities;

namespace Modmail.NET.Common;

public static class UtilPermission
{
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
      overwrites.Add(memberPerm);
    }

    foreach (var role in roles) {
      var rolePerm = new DiscordOverwriteBuilder(role);
      rolePerm.Allow(Permissions.AccessChannels);
      rolePerm.Allow(Permissions.ReadMessageHistory);
      rolePerm.Allow(Permissions.SendMessages);
      rolePerm.Allow(Permissions.EmbedLinks);
      rolePerm.Allow(Permissions.AttachFiles);
      overwrites.Add(rolePerm);
    }

    return overwrites;
  }
}