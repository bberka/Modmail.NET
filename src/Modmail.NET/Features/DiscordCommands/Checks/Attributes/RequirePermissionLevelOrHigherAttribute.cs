using DSharpPlus.Commands.ContextChecks;
using Modmail.NET.Features.Permission.Static;

namespace Modmail.NET.Features.DiscordCommands.Checks.Attributes;

public class RequirePermissionLevelOrHigherAttribute : ContextCheckAttribute
{
  public RequirePermissionLevelOrHigherAttribute(TeamPermissionLevel teamPermissionLevel) {
    TeamPermissionLevel = teamPermissionLevel;
  }

  public TeamPermissionLevel TeamPermissionLevel { get; }
}