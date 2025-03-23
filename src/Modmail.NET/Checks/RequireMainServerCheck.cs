using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Checks.Attributes;

namespace Modmail.NET.Checks;

public class RequireMainServerCheck : IContextCheck<RequireMainServerAttribute>
{
  public async ValueTask<string> ExecuteCheckAsync(RequireMainServerAttribute attribute, CommandContext context) {
    var config = context.ServiceProvider.GetRequiredService<IOptions<BotConfig>>().Value;
    var isMainServer = config.MainServerId == context.Guild?.Id;
    if (isMainServer) return null;

    return await Task.FromResult(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation());
  }
}