using DSharpPlus.Commands.ContextChecks;

namespace Modmail.NET.Checks.Attributes;

public sealed class RequirePermissionLevelOrHigherAttribute : ContextCheckAttribute
{
  public TeamPermissionLevel TeamPermissionLevel { get; }

  public RequirePermissionLevelOrHigherAttribute(TeamPermissionLevel teamPermissionLevel) {
    TeamPermissionLevel = teamPermissionLevel;
  }
}