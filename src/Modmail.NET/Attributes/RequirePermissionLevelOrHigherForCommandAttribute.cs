using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Attributes;

public class RequirePermissionLevelOrHigherForCommandAttribute : CheckBaseAttribute
{
  private readonly TeamPermissionLevel _teamPermissionLevel;

  public RequirePermissionLevelOrHigherForCommandAttribute(TeamPermissionLevel teamPermissionLevel) {
    _teamPermissionLevel = teamPermissionLevel;
  }

  public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) {
    var isOwner = BotConfig.This.OwnerUsers.Contains(ctx.User.Id);
    if (isOwner) return true;

    var guild = ctx.Guild;
    if (guild == null) {
      await ctx.RespondAsync(Embeds.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()));
      return false;
    }

    if (ctx.Member is null) {
      await ctx.RespondAsync(Embeds.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()));//TODO: Add another message
      return false;
    }
    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) return true;

    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await GuildTeamMember.GetPermissionLevelAsync(ctx.User.Id, roleIdList);
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