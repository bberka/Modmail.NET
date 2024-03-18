using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Static;

namespace Modmail.NET.Attributes;

public class RequireAdminAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) {
      return true;
    }

    var guild = ctx.Guild;
    if (guild is null) {
      return false;
    }

    var dbService = ctx.Services.GetService<IDbService>();
     
    var roleIdList = ctx.Member.Roles.Select(x => x.Id).ToList();
    var permLevel = await dbService!.GetPermissionLevelAsync(ctx.User.Id, guild.Id, roleIdList);
    if (permLevel == TeamPermissionLevel.Admin) {
      return true;
    }

 
    return false;
  }
}