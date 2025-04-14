using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.DiscordCommands.Checks;

public class RequireModmailPermissionCheck : IContextCheck<RequireModmailPermissionAttribute>
{
  public async ValueTask<string?> ExecuteCheckAsync(RequireModmailPermissionAttribute attribute, CommandContext context) {
    var scope = context.ServiceProvider.CreateScope();
    var config = scope.ServiceProvider.GetRequiredService<IOptions<BotConfig>>().Value;
    var isOwner = config.SuperUsers.Contains(context.User.Id);
    if (isOwner) return null;

    var guild = context.Guild?.Id == config.MainServerId
                  ? context.Guild
                  : null;
    if (guild is null) return await Task.FromResult(Lang.ThisCommandCanOnlyBeUsedInMainServer.Translate());

    if (context.Member is null)
      return await Task.FromResult(Lang.YouDoNotHavePermissionToUseThisCommand.Translate());

    var isAdmin = context.Member.Permissions.HasPermission(DiscordPermission.Administrator);
    if (isAdmin) return null;

    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var anyTeamMember = await sender.Send(new CheckUserInAnyTeamQuery(context.User.Id));
    if (!anyTeamMember) return await Task.FromResult(Lang.YouDoNotHavePermissionToUseThisCommand.Translate());

    if (attribute.AuthPolicy is null) return null; // if auth policy is null but attribute is present, only perm needed is to be in a team

    var canAccess = await sender.Send(new CheckPermissionAccessQuery(context.User.Id, attribute.AuthPolicy));
    if (!canAccess) return await Task.FromResult(Lang.YouDoNotHavePermissionToUseThisCommand.Translate());

    return null;
  }
}