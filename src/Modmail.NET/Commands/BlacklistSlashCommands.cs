using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
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
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    await DiscordUserInfo.AddOrUpdateAsync(user);
    try {
      await TicketBlacklist.ProcessAddUserToBlacklist(ctx.User.Id, user.Id, reason, notifyUser);
      await ctx.EditResponseAsync(Webhooks.Success(Texts.USER_BLACKLISTED));
    }
    catch (BotExceptionBase exception) {
      Log.Error(exception, "An exception occurred while adding a user to the blacklist");
      await ctx.EditResponseAsync(Webhooks.Warning(exception.TitleMessage, exception.ContentMessage ?? ""));
    }
    catch (Exception e) {
      Log.Error(e, "An exception occurred while adding a user to the blacklist");
      await ctx.EditResponseAsync(Webhooks.Error(Texts.AN_EXCEPTION_OCCURRED));
    }
  }

  [SlashCommand("remove", "Remove a user from the blacklist.")]
  public async Task Remove(InteractionContext ctx,
                           [Option("user", "The user to remove from the blacklist.")]
                           DiscordUser user,
                           [Option("notify-user", "Whether to notify the user about the removal.")]
                           bool notifyUser = true
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    await DiscordUserInfo.AddOrUpdateAsync(user);

    var option = await GuildOption.GetAsync();
    if (option is null) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.SERVER_NOT_SETUP));
      return;
    }

    var logChannelId = option.LogChannelId;
    var logChannel = ctx.Guild.GetChannel(logChannelId);

    if (logChannel is null) {
      await ctx.EditResponseAsync(Webhooks.Error(Texts.SERVER_NOT_SETUP));
      return;
    }

    var ticketBlacklist = await TicketBlacklist.GetAsync(user.Id);
    if (ticketBlacklist is null) {
      await ctx.EditResponseAsync(Webhooks.Success(Texts.USER_IS_NOT_BLACKLISTED));
      return;
    }

    await ticketBlacklist.RemoveAsync();

    var embedLog = EmbedLog.BlacklistRemoved(ctx.User, user);
    await logChannel.SendMessageAsync(embedLog);
    var builderLog = new DiscordWebhookBuilder().AddEmbed(embedLog);
    await ctx.EditResponseAsync(builderLog);


    if (notifyUser) {
      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
      if (member is not null) {
        var dmEmbed = EmbedUser.YouHaveBeenRemovedFromBlacklist(ctx.User);
        await member.SendMessageAsync(dmEmbed);
      }
    }
  }

  [SlashCommand("status", "Check if a user is blacklisted.")]
  public async Task Status(InteractionContext ctx,
                           [Option("user", "The user to check.")] DiscordUser user
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    var isBlocked = await TicketBlacklist.IsBlacklistedAsync(user.Id);
    await ctx.EditResponseAsync(Webhooks.Info(Texts.USER_BLACKLIST_STATUS,
                                              isBlocked
                                                ? Texts.USER_IS_BLACKLISTED
                                                : Texts.USER_IS_NOT_BLACKLISTED));
  }

  [SlashCommand("view", "View all blacklisted users.")]
  public async Task View(InteractionContext ctx) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    var blacklistedUsers = (await TicketBlacklist.GetAllAsync()).Select(x => $"<@{x}>").ToList();
    if (blacklistedUsers.Count == 0) {
      await ctx.EditResponseAsync(Webhooks.Info(Texts.NO_BLACKLISTED_USERS));
      return;
    }

    await ctx.EditResponseAsync(Webhooks.Info(Texts.BLACKLISTED_USERS, string.Join("\n", blacklistedUsers)));
  }
}