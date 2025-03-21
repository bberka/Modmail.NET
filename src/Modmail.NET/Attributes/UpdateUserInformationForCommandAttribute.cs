using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Attributes;

public sealed class UpdateUserInformationForCommandAttribute : CheckBaseAttribute
{
  public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) {
    var sender = ctx.Services.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(ctx.User));
    return true;
  }
}