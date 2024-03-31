using DSharpPlus;
using DSharpPlus.SlashCommands;
using Modmail.NET.Common;
using Modmail.NET.Entities;

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
    if (guild is null) {
      await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                Interactions.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()).AsEphemeral());
      return false;
    }


    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await GuildTeamMember.GetPermissionLevelAsync(ctx.User.Id, roleIdList);
    if (permLevel is null) {
      await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                Interactions.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()).AsEphemeral());
      return false;
    }

    var permLevelInt = (int)permLevel;
    var expectedLevelInt = (int)_teamPermissionLevel;
    if (permLevelInt < expectedLevelInt) {
      await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                Interactions.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()).AsEphemeral());
      return false;
    }

    return true;
  }
}