using DSharpPlus.Commands.ContextChecks;

namespace Modmail.NET.Checks.Attributes;

public class RequirePermissionLevelOrHigherAttribute : ContextCheckAttribute
{
  public RequirePermissionLevelOrHigherAttribute(TeamPermissionLevel teamPermissionLevel) {
    TeamPermissionLevel = teamPermissionLevel;
  }

  public TeamPermissionLevel TeamPermissionLevel { get; }
}