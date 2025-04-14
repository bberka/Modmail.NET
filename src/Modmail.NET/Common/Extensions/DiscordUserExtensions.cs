using DSharpPlus.Entities;

namespace Modmail.NET.Common.Extensions;

public static class DiscordUserExtensions
{
  public static string GetUsername(this DiscordUser? user) {
    if (user is null) return string.Empty;
    return user.Discriminator == "0" || string.IsNullOrEmpty(user.Discriminator)
             ? user.Username
             : $"{user.Username}#{user.Discriminator}";
  }

  public static string GetUsername(this DiscordMember? member) {
    if (member is null) return string.Empty;
    return member.Discriminator == "0" || string.IsNullOrEmpty(member.Discriminator)
             ? member.Username
             : $"{member.Username}#{member.Discriminator}";
  }
}