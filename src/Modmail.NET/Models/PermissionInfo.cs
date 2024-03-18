using Modmail.NET.Static;

namespace Modmail.NET.Models;

public record PermissionInfo(TeamPermissionLevel PermissionLevel, ulong Key, TeamMemberDataType Type);