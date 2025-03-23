using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Aspects;
using Modmail.NET.Checks.Attributes;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Features.Blacklist;
using Modmail.NET.Features.UserInfo;
using Serilog;

namespace Modmail.NET.Commands.Slash;

[PerformanceLoggerAspect]
[Command("blacklist")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
[UpdateUserInformation]
public class BlacklistSlashCommands
{
  private readonly ISender _sender;

  public BlacklistSlashCommands(ISender sender) {
    _sender = sender;
  }

  [Command("add")]
  [Description("Adds a user to the blacklist")]
  [SlashCommandTypes(DiscordApplicationCommandType.SlashCommand)]
  public async Task Add(SlashCommandContext ctx,
                        [Parameter("user")] [Description("The user to blacklist")]
                        DiscordUser user,
                        [Parameter("reason")] [Description("The reason for blacklisting")]
                        string reason = "No reason provided."
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{Reason}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await _sender.Send(new UpdateDiscordUserCommand(user));
      await _sender.Send(new ProcessAddUserToBlacklistCommand(ctx.User.Id, user.Id, reason));
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

  [Command("remove")]
  [Description("Removes a user from the blacklist.")]
  [SlashCommandTypes(DiscordApplicationCommandType.SlashCommand)]
  public async Task Remove(SlashCommandContext ctx,
                           [Parameter("user")] [Description("The user to remove from the blacklist.")]
                           DiscordUser user
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Remove)}({{ContextUserId}},{{UserId}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await _sender.Send(new UpdateDiscordUserCommand(user));
      await _sender.Send(new ProcessRemoveUserFromBlacklistCommand(user.Id, ctx.User.Id));
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

  [Command("status")]
  [Description("Check if a user is blacklisted.")]
  public async Task Status(SlashCommandContext ctx,
                           [Parameter("user")] [Description("The user to check.")]
                           DiscordUser user
  ) {
    const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Status)}({{ContextUserId}},{{UserId}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var isBlocked = await _sender.Send(new CheckUserBlacklistStatusQuery(ctx.User.Id,user.Id));
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