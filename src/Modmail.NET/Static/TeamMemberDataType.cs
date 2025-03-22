using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace Modmail.NET.Static;

public enum TeamMemberDataType
{
  [ChoiceDisplayName("Role")]
  RoleId,

  [ChoiceDisplayName("User")]
  UserId
}