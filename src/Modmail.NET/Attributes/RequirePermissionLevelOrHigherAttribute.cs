﻿using DSharpPlus;
using DSharpPlus.SlashCommands;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Attributes;

public class RequirePermissionLevelOrHigherAttribute : SlashCheckBaseAttribute
{
  private readonly TeamPermissionLevel _teamPermissionLevel;

  public RequirePermissionLevelOrHigherAttribute(TeamPermissionLevel teamPermissionLevel) {
    _teamPermissionLevel = teamPermissionLevel;
  }

  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var isOwner = BotConfig.This.OwnerUsers.Contains(ctx.User.Id);
    if (isOwner) return true;

    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) return true;

    var guild = ctx.Guild;
    if (guild is null) return false;


    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await GuildTeamMember.GetPermissionLevelAsync(ctx.User.Id, guild.Id, roleIdList);
    if (permLevel is null) return false;
    var permLevelInt = (int)permLevel;
    var expectedLevelInt = (int)_teamPermissionLevel;
    if (permLevelInt >= expectedLevelInt) return true;

    return false;
  }
}