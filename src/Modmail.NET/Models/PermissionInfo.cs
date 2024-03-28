using Modmail.NET.Static;

namespace Modmail.NET.Models;

public record PermissionInfo(TeamPermissionLevel PermissionLevel, ulong Key, TeamMemberDataType Type, bool PingOnNewTicket, bool PingOnNewMessage)
{
  public string GetMention() => Type switch {
    TeamMemberDataType.RoleId => $"<@&{Key}>",
    TeamMemberDataType.UserId => $"<@{Key}>",
    _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
  };
}