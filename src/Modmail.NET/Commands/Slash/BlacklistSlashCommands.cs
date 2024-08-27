using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Aspects;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Serilog;

namespace Modmail.NET.Commands.Slash;

[PerformanceLoggerAspect]
[SlashCommandGroup("blacklist", "Blacklist management commands.")]
[RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Moderator)]
[UpdateUserInformationForSlash]
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
      await ctx.EditResponseAsync(Webhooks.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
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
      await ctx.EditResponseAsync(Webhooks.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
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
      await ctx.EditResponseAsync(Webhooks.Info(LangKeys.USER_BLACKLIST_STATUS.GetTranslation(),
                                                isBlocked
                                                  ? LangKeys.USER_IS_BLACKLISTED.GetTranslation()
                                                  : LangKeys.USER_IS_NOT_BLACKLISTED.GetTranslation()));
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
      await ctx.EditResponseAsync(Webhooks.Info(LangKeys.BLACKLISTED_USERS.GetTranslation(), str));
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