using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Attributes;

public sealed class UpdateUserInformationForSlashAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var sender = ctx.Services.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(ctx.User));
    return true;
  }
}