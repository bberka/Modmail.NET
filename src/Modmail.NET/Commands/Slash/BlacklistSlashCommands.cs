using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Modmail.NET.Aspects;
using Modmail.NET.Attributes;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Features.Blacklist;
using Modmail.NET.Features.UserInfo;
using Serilog;

namespace Modmail.NET.Commands.Slash;

[PerformanceLoggerAspect]
[SlashCommandGroup("blacklist", "Blacklist management commands.")]
[RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Moderator)]
[UpdateUserInformationForSlash]
[ModuleLifespan(ModuleLifespan.Transient)]
public class BlacklistSlashCommands : ApplicationCommandModule
{
  private readonly ISender _sender;

  public BlacklistSlashCommands(ISender sender) {
    _sender = sender;
  }

  [SlashCommand("add", "Add a user to the blacklist.")]
  public async Task Add(InteractionContext ctx,
                        [Option("user", "The user to blacklist.")]
                        DiscordUser user,
                        [Option("reason", "The reason for blacklisting.")]
                        string reason = "No reason provided."
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{Reason}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await _sender.Send(new UpdateDiscordUserCommand(user));
      await _sender.Send(new ProcessAddUserToBlacklistCommand(user.Id, reason, ctx.User.Id));
      await ctx.EditResponseAsync(Webhooks.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
      Log.Information(logMessage,
                      ctx.User.Id,
                      user.Id,
                      reason);
    }
    catch (BotExceptionBase ex) {
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  user.Id,
                  reason);
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
    }
    catch (Exception ex) {
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                user.Id,
                reason);
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
    }
  }

  [SlashCommand("remove", "Remove a user from the blacklist.")]
  public async Task Remove(InteractionContext ctx,
                           [Option("user", "The user to remove from the blacklist.")]
                           DiscordUser user
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Remove)}({{ContextUserId}},{{UserId}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await _sender.Send(new UpdateDiscordUserCommand(user));
      await _sender.Send(new ProcessRemoveUserFromBlacklistCommand(ctx.User.Id, user.Id));
      Log.Information(logMessage,
                      ctx.User.Id,
                      user.Id);
      await ctx.EditResponseAsync(Webhooks.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  user.Id);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                user.Id);
    }
  }

  [SlashCommand("status", "Check if a user is blacklisted.")]
  public async Task Status(InteractionContext ctx,
                           [Option("user", "The user to check.")] DiscordUser user
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Status)}({{ContextUserId}},{{UserId}})";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var isBlocked = await _sender.Send(new CheckUserBlacklistStatusQuery(user.Id));
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
}