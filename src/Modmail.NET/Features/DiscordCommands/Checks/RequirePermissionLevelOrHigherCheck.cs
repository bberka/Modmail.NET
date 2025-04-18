using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.DiscordCommands.Checks;

public class RequirePermissionLevelOrHigherCheck : IContextCheck<RequirePermissionLevelOrHigherAttribute>
{
  public async ValueTask<string> ExecuteCheckAsync(RequirePermissionLevelOrHigherAttribute attribute, CommandContext context) {
    var scope = context.ServiceProvider.CreateScope();
    var config = scope.ServiceProvider.GetRequiredService<IOptions<BotConfig>>().Value;
    var isOwner = config.OwnerUsers.Contains(context.User.Id);
    if (isOwner) return null;

    var guild = context.Guild?.Id == config.MainServerId
                  ? context.Guild
                  : null;
    if (guild is null) return await Task.FromResult(LangKeys.ThisCommandCanOnlyBeUsedInMainServer.GetTranslation());

    if (context.Member is null)
      return await Task.FromResult(LangKeys.YouDoNotHavePermissionToUseThisCommand.GetTranslation());

    var isAdmin = context.Member.Permissions.HasPermission(DiscordPermission.Administrator);
    if (isAdmin) return null;

    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var permLevel = await sender.Send(new GetPermissionLevelQuery(context.User.Id, true));
    if (permLevel is null) return await Task.FromResult(LangKeys.YouDoNotHavePermissionToUseThisCommand.GetTranslation());

    var permLevelInt = (int)permLevel;
    var expectedLevelInt = (int)attribute.TeamPermissionLevel;
    if (permLevelInt < expectedLevelInt) return await Task.FromResult(LangKeys.YouDoNotHavePermissionToUseThisCommand.GetTranslation());


    return null;
  }
}