using DSharpPlus.SlashCommands;

namespace Modmail.NET.Static;

public enum TeamMemberDataType
{
  [ChoiceName("Role")]
  RoleId,

  [ChoiceName("User")]
  UserId
}