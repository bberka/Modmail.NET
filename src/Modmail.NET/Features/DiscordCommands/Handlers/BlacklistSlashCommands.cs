﻿using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Blacklist.Commands;
using Modmail.NET.Features.Blacklist.Queries;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Permission.Static;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordCommands.Handlers;

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
      await ctx.EditResponseAsync(ModmailWebhooks.Success(LangKeys.UserBlacklisted.GetTranslation()));
      Log.Information(logMessage,
                      ctx.User.Id,
                      user.Id,
                      reason);
    }
    catch (ModmailBotException ex) {
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
      await ctx.EditResponseAsync(ModmailWebhooks.Success(LangKeys.UserBlacklisted.GetTranslation()));
    }
    catch (ModmailBotException ex) {
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
      var isBlocked = await _sender.Send(new CheckUserBlacklistStatusQuery(user.Id));
      await ctx.EditResponseAsync(ModmailWebhooks.Info(LangKeys.UserBlacklistStatus.GetTranslation(),
                                                       isBlocked
                                                         ? LangKeys.UserIsBlacklisted.GetTranslation()
                                                         : LangKeys.UserIsNotBlacklisted.GetTranslation()));
      Log.Information(logMessage, ctx.User.Id, user.Id);
    }
    catch (ModmailBotException ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, user.Id);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, user.Id);
    }
  }
}