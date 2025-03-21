using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Modmail.NET.Attributes;

public sealed class RequireMainServerForCommandAttribute : CheckBaseAttribute
{
  public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) {
    var config = ctx.Services.GetRequiredService<IOptions<BotConfig>>().Value;
    var isMainServer = config.MainServerId == ctx.Guild.Id;
    if (isMainServer) return Task.FromResult(true);

    ctx.RespondAsync(Embeds.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()));
    return Task.FromResult(false);
  }
}