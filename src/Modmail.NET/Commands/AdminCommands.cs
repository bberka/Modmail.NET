using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Handlers;
using Serilog;

namespace Modmail.NET.Commands;

[RequireBotPermissions(Permissions.SendMessages)]
public class AdminCommands : BaseCommandModule
{
  [Command("setup")]
  public async Task Setup(CommandContext ctx) {
    var isPrivate = ctx.Channel.IsPrivate;
    if (isPrivate) {
      await ctx.Channel.SendMessageAsync("This command can only be used in a server!");
      Log.Warning("User {User} tried to use setup command in DM", ctx.User.Username);
      return;
    }

    var isBotOwner = EnvContainer.This.OwnerUsers.Contains(ctx.User.Id);
    var isAuthorAdmin = ctx.Member?.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator);
    if (!isAuthorAdmin.HasValue && !isBotOwner) {
      await ctx.Channel.SendMessageAsync("You must be an administrator or bot owner to use this command!");
      Log.Warning("User {User} tried to use setup command without permission", ctx.User.Username);
      return;
    }

    var msg = await ctx.Channel.SendMessageAsync("Setting up server!, Please wait...");
    await using var dbContext = new ModmailDbContext();
    try {
      await dbContext.Database.EnsureCreatedAsync();
      await dbContext.Database.MigrateAsync();
      Log.Information("Database migration completed!");
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to setup server!");
      await msg.ModifyAsync("Failed to setup server, please check bot logs!");
      return;
    }

    var mainGuild = await ctx.Client.GetGuildAsync((ulong)EnvContainer.This.MainServerId);
    if (mainGuild is null) {
      await msg.ModifyAsync("Failed to setup server, please check bot logs!");
      Log.Error("Failed to get main server! Please check the MainServerId in .env file");
      return;
    }

    //create a channel 
    var category = await mainGuild.CreateChannelCategoryAsync("Modmail");

    //create a log channel
    var logChannel = await mainGuild.CreateTextChannelAsync("modmail-logs", category);

    var mmOption = new ModmailOption() {
      CategoryId = (long)category.Id,
      GuildId = (long)mainGuild.Id,
      LogChannelId = (long)logChannel.Id,
      IsListenPrivateMessages = true,
    };
    dbContext.ModmailOptions.Add(mmOption);
    await dbContext.SaveChangesAsync();


    await msg.ModifyAsync("Successfully setup server!");
    Log.Information("Server setup completed!");
  }
}