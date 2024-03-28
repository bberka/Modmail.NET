using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("blacklist", "Blacklist management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
[UpdateUserInformation]
public class BlacklistSlashCommands : ApplicationCommandModule
{
  [SlashCommand("add", "Add a user to the blacklist.")]
  public async Task Add(InteractionContext ctx,
                        [Option("user", "The user to blacklist.")]
                        DiscordUser user,
                        [Option("notify-user", "Whether to notify the user about the blacklist.")]
                        bool notifyUser = true,
                        [Option("reason", "The reason for blacklisting.")]
                        string reason = "No reason provided."
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{NotifyUser}},{{Reason}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await DiscordUserInfo.AddOrUpdateAsync(user);
      await TicketBlacklist.ProcessAddUserToBlacklist(ctx.User.Id, user.Id, reason, notifyUser);
      await ctx.EditResponseAsync(Webhooks.Success(Texts.USER_BLACKLISTED));
      Log.Information(logMessage,
                      ctx.User.Id,
                      user.Id,
                      notifyUser,
                      reason);
    }
    catch (BotExceptionBase ex) {
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  user.Id,
                  notifyUser,
                  reason);
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
    }
    catch (Exception ex) {
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                user.Id,
                notifyUser,
                reason);
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
    }
  }

  [SlashCommand("remove", "Remove a user from the blacklist.")]
  public async Task Remove(InteractionContext ctx,
                           [Option("user", "The user to remove from the blacklist.")]
                           DiscordUser user,
                           [Option("notify-user", "Whether to notify the user about the removal.")]
                           bool notifyUser = true
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Remove)}({{ContextUserId}},{{UserId}},{{NotifyUser}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await DiscordUserInfo.AddOrUpdateAsync(user);
      var ticketBlacklist = await TicketBlacklist.GetAsync(user.Id);
      await ticketBlacklist.ProcessRemoveUserFromBlacklist(ctx.User.Id, user.Id, notifyUser);
      Log.Information(logMessage,
                      ctx.User.Id,
                      user.Id,
                      notifyUser);
      await ctx.EditResponseAsync(Webhooks.Success(Texts.USER_BLACKLISTED));
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  user.Id,
                  notifyUser);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                user.Id,
                notifyUser);
    }
  }

  [SlashCommand("status", "Check if a user is blacklisted.")]
  public async Task Status(InteractionContext ctx,
                           [Option("user", "The user to check.")] DiscordUser user
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Status)}({{ContextUserId}},{{UserId}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var isBlocked = await TicketBlacklist.IsBlacklistedAsync(user.Id);
      await ctx.EditResponseAsync(Webhooks.Info(Texts.USER_BLACKLIST_STATUS,
                                                isBlocked
                                                  ? Texts.USER_IS_BLACKLISTED
                                                  : Texts.USER_IS_NOT_BLACKLISTED));
      Log.Information(logMessage, ctx.User.Id, user.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, user.Id);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, user.Id);
    }
  }

  [SlashCommand("view", "View all blacklisted users.")]
  public async Task View(InteractionContext ctx) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(View)}({{ContextUserId}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var blacklistedUsers = await TicketBlacklist.GetAllAsync();
      var str = string.Join("\n", blacklistedUsers.Select(x => $"<@{x.DiscordUserId}>").ToList());
      await ctx.EditResponseAsync(Webhooks.Info(Texts.BLACKLISTED_USERS, str));
      Log.Information(logMessage, ctx.User.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id);
    }
  }
}