using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Checks.Attributes;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Checks;

public class UpdateUserInformationCheck : IContextCheck<UpdateUserInformationAttribute>
{
  public async ValueTask<string> ExecuteCheckAsync(UpdateUserInformationAttribute attribute, CommandContext context) {
    var scope = context.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(context.User));
    return null;
  }
}