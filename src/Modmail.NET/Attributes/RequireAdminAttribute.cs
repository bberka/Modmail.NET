using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace Modmail.NET.Attributes;

public class RequireAdminAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var isAdmin = ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    if (isAdmin) {
      return true;
    }

    return false;
    // //return ctx.User.Id == 337045211362623493;
    // var userRepository = ctx.Services.GetService<IUserRepository>();
    // var isAdminUser = await userRepository!.IsAdminUser(ctx.User.Id);
    // Console.WriteLine($"User {(isAdminUser ? "is" : "is not")} admin");
    // return isAdminUser;
  }
}