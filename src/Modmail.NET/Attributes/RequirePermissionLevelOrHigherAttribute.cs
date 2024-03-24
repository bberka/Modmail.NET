using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Static;

namespace Modmail.NET.Attributes;

public class RequirePermissionLevelOrHigherAttribute : SlashCheckBaseAttribute
{
  private readonly TeamPermissionLevel _teamPermissionLevel;

  public RequirePermissionLevelOrHigherAttribute(TeamPermissionLevel teamPermissionLevel) {
    _teamPermissionLevel = teamPermissionLevel;
  }

  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var isOwner = MMConfig.This.OwnerUsers.Contains(ctx.User.Id);
    if (isOwner) return true;

    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) return true;

    var guild = ctx.Guild;
    if (guild is null) return false;

    var dbService = ctx.Services.GetService<IDbService>();

    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await dbService!.GetPermissionLevelAsync(ctx.User.Id, guild.Id, roleIdList);
    if (permLevel is null) return false;
    var permLevelInt = (int)permLevel;
    var expectedLevelInt = (int)_teamPermissionLevel;
    if (permLevelInt >= expectedLevelInt) return true;

    return false;
  }
}