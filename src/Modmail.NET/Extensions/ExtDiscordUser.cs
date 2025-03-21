using DSharpPlus.Entities;

namespace Modmail.NET.Extensions;

public static class ExtDiscordUser
{
  //implement caching for log channel etc.

  public static string GetUsername(this DiscordUser user) {
    if (user == null) return string.Empty;
    return user.Discriminator == "0" || string.IsNullOrEmpty(user.Discriminator)
             ? user.Username
             : $"{user.Username}#{user.Discriminator}";
  }

  public static string GetUsername(this DiscordMember member) {
    if (member == null) return string.Empty;
    return member.Discriminator == "0" || string.IsNullOrEmpty(member.Discriminator)
             ? member.Username
             : $"{member.Username}#{member.Discriminator}";
  }
}