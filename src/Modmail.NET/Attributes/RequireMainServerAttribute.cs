using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Modmail.NET.Attributes;

public sealed class RequireMainServerForSlashCommandAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var config = ctx.Services.GetRequiredService<IOptions<BotConfig>>().Value;
    var isMainServer = config.MainServerId == ctx.Guild.Id;
    if (isMainServer) return true;

    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                              Interactions.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()).AsEphemeral());
    return false;
  }
}