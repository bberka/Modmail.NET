using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace Modmail.NET.Features.Teams.Static;

public enum TeamMemberDataType
{
  [ChoiceDisplayName("Role")]
  RoleId,

  [ChoiceDisplayName("User")]
  UserId
}