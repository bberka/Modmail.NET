using DSharpPlus;
using DSharpPlus.SlashCommands;
using Modmail.NET.Common;
using Modmail.NET.Static;

namespace Modmail.NET.Attributes;

public class RequireMainServerAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var isMainServer = BotConfig.This.MainServerId == ctx.Guild.Id;
    if (isMainServer) {
      return true;
    }

    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, Interactions.Error(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER));
    return false;
  }
}