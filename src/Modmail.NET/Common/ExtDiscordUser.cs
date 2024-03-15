using DSharpPlus;
using DSharpPlus.Entities;

namespace Modmail.NET.Common;

public static class ExtDiscordUser
{
    //implement caching for log channel etc.

    public static string GetUsername(this DiscordUser user) {
      return user.Discriminator == "0"
               ? user.Username
               : $"{user.Username}#{user.Discriminator}";
    }

    public static string GetUsername(this DiscordMember member) {
      return member.Discriminator == "0"
               ? member.Username
               : $"{member.Username}#{member.Discriminator}";
    }

}