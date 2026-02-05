using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.User.Commands;

namespace Modmail.NET.Features.DiscordCommands.Checks;

public class UpdateUserInformationCheck : IContextCheck<UpdateUserInformationAttribute>
{
    public async ValueTask<string?> ExecuteCheckAsync(UpdateUserInformationAttribute attribute, CommandContext context)
    {
        var scope = context.ServiceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new UpdateDiscordUserCommand(context.User));
        return null;
    }
}