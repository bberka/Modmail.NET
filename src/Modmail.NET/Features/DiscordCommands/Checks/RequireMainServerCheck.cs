using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Language;

namespace Modmail.NET.Features.DiscordCommands.Checks;

public class RequireMainServerCheck : IContextCheck<RequireMainServerAttribute>
{
    public async ValueTask<string?> ExecuteCheckAsync(RequireMainServerAttribute attribute, CommandContext context)
    {
        var config = context.ServiceProvider.GetRequiredService<IOptions<BotConfig>>()
            .Value;
        var isMainServer = config.MainServerId == context.Guild?.Id;
        if (isMainServer) return null;

        return await Task.FromResult(Lang.ThisCommandCanOnlyBeUsedInMainServer.Translate());
    }
}