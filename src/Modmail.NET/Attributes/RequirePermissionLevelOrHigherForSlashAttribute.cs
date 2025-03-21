using DSharpPlus;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Attributes;

public sealed class RequirePermissionLevelOrHigherForSlashAttribute : SlashCheckBaseAttribute
{
  private readonly TeamPermissionLevel _teamPermissionLevel;

  public RequirePermissionLevelOrHigherForSlashAttribute(TeamPermissionLevel teamPermissionLevel) {
    _teamPermissionLevel = teamPermissionLevel;
  }

  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var config = ctx.Services.GetRequiredService<IOptions<BotConfig>>().Value;
    var isOwner = config.OwnerUsers.Contains(ctx.User.Id);
    if (isOwner) return true;

    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) return true;

    var guild = ctx.Guild is not null && ctx.Guild.Id == config.MainServerId
                  ? ctx.Guild
                  : null;
    if (guild is null) {
      await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                Interactions.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()).AsEphemeral());
      return false;
    }

    var scope = ctx.Services.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await sender.Send(new GetTeamPermissionLevelQuery(ctx.User.Id, roleIdList));
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