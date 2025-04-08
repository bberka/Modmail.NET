using Modmail.NET.Features.Permission.Static;
using Modmail.NET.Features.Teams.Static;

namespace Modmail.NET.Features.Permission.Models;

public sealed record PermissionInfo(TeamPermissionLevel PermissionLevel, ulong Key, TeamMemberDataType Type, bool PingOnNewTicket, bool PingOnNewMessage)
{
  public string GetMention() {
    return Type switch {
      TeamMemberDataType.RoleId => $"<@&{Key}>",
      TeamMemberDataType.UserId => $"<@{Key}>",
      _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
    };
  }
}