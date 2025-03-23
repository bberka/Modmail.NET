using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;

namespace Modmail.NET.Language;

public static class StringInterpolationMgr
{
  private const string NEW_LINE_INTERPOLATION = "{NewLine}";

  public static string Build(string text,
                             ulong? userId = null,
                             ulong? modUserId = null,
                             ulong? channelId = null,
                             ulong? guildId = null,
                             string userName = null,
                             string modUserName = null,
                             string channelName = null,
                             string guildName = null,
                             string roleName = null,
                             string teamName = null,
                             string ticketId = null
  ) {
    var sb = new StringBuilder(text);
    return sb.Replace("{UserId}", userId?.ToString() ?? "Unknown")
             .Replace("{ModUserId}", modUserId?.ToString() ?? "Unknown")
             .Replace("{ChannelId}", channelId?.ToString() ?? "Unknown")
             .Replace("{GuildId}", guildId?.ToString() ?? "Unknown")
             .Replace("{UserName}", userName ?? "Unknown")
             .Replace("{ModUserName}", modUserName ?? "Unknown")
             .Replace("{ChannelName}", channelName ?? "Unknown")
             .Replace("{GuildName}", guildName ?? "Unknown")
             .Replace("{RoleName}", roleName ?? "Unknown")
             .Replace("{TeamName}", teamName ?? "Unknown")
             .Replace("{TicketId}", ticketId ?? "Unknown")
             .Replace(NEW_LINE_INTERPOLATION, Environment.NewLine)
             .ToString();
  }


  public static string Build(string text,
                             DiscordGuild guild = null,
                             DiscordUser user = null,
                             DiscordUser modUser = null,
                             DiscordChannel channel = null,
                             DiscordRole role = null,
                             GuildTeam team = null
  ) {
    return Build(text,
                 user?.Id,
                 modUser?.Id,
                 channel?.Id,
                 guild?.Id,
                 user?.Username,
                 modUser?.Username,
                 channel?.Name,
                 guild?.Name,
                 role?.Name,
                 team?.Name);
  }

  public static string Build(string text,
                             GuildOption guild = null,
                             DiscordUserInfo user = null,
                             DiscordUserInfo modUser = null,
                             DiscordChannel channel = null,
                             DiscordRole role = null,
                             GuildTeam team = null
  ) {
    return Build(text,
                 user?.Id,
                 modUser?.Id,
                 channel?.Id,
                 guild?.GuildId,
                 user?.Username,
                 modUser?.Username,
                 channel?.Name,
                 guild?.Name,
                 role?.Name,
                 team?.Name);
  }
}