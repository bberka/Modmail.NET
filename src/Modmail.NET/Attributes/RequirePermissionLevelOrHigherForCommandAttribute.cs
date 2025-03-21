﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Attributes;

public sealed class RequirePermissionLevelOrHigherForCommandAttribute : CheckBaseAttribute
{
  private readonly TeamPermissionLevel _teamPermissionLevel;

  public RequirePermissionLevelOrHigherForCommandAttribute(TeamPermissionLevel teamPermissionLevel) {
    _teamPermissionLevel = teamPermissionLevel;
  }

  public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) {
    var config = ctx.Services.GetRequiredService<IOptions<BotConfig>>().Value;
    var isOwner = config.OwnerUsers.Contains(ctx.User.Id);
    if (isOwner) return true;

    var guild = ctx.Guild.Id == config.MainServerId
                  ? ctx.Guild
                  : null;
    if (guild == null) {
      await ctx.RespondAsync(Embeds.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()));
      return false;
    }

    if (ctx.Member is null) {
      await ctx.RespondAsync(Embeds.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation())); //TODO: Add another message
      return false;
    }

    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) return true;

    var scope = ctx.Services.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await sender.Send(new GetTeamPermissionLevelQuery(ctx.User.Id, roleIdList));
    if (permLevel is null) {
      await ctx.RespondAsync(Embeds.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()));
      return false;
    }

    var permLevelInt = (int)permLevel;
    var expectedLevelInt = (int)_teamPermissionLevel;
    if (permLevelInt < expectedLevelInt) {
      await ctx.RespondAsync(Embeds.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()));
      return false;
    }


    return true;
  }
}