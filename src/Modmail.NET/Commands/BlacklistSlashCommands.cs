﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("blacklist", "Blacklist management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
public class BlacklistSlashCommands : ApplicationCommandModule
{
  [SlashCommand("add", "Add a user to the blacklist.")]
  public async Task Add(InteractionContext ctx,
                        [Option("user", "The user to blacklist.")]
                        DiscordUser user,
                        [Option("notify-user", "Whether to notify the user about the blacklist.")]
                        bool notifyUser,
                        [Option("reason", "The reason for blacklisting.")]
                        string reason
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var dbService = ServiceLocator.Get<IDbService>();

    var option = await dbService.GetOptionAsync(ctx.Guild.Id);
    if (option is null) {
      var embed1 = ModmailEmbedBuilder.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder1 = new DiscordWebhookBuilder().AddEmbed(embed1);
      await ctx.EditResponseAsync(builder1);
      return;
    }

    var logChannelId = option.LogChannelId;
    var logChannel = ctx.Guild.GetChannel(logChannelId);

    if (logChannel is null) {
      var embed2 = ModmailEmbedBuilder.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.EditResponseAsync(builder2);
      return;
    }

    var activeTicket = await dbService.GetActiveTicketAsync(user.Id);
    if (activeTicket is not null) {
      var embed3 = ModmailEmbedBuilder.Base("User has an active ticket!", "Please close the ticket before blacklisting the user.", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.EditResponseAsync(builder);
      return;
    }


    var activeBlock = await dbService.GetUserBlacklistStatus(user.Id);
    if (activeBlock) {
      var embed4 = ModmailEmbedBuilder.Base("User is already blacklisted!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.EditResponseAsync(builder);
      return;
    }

    await dbService.UpdateUserInfoAsync(new DiscordUserInfo(ctx.User));
    await dbService.UpdateUserInfoAsync(new DiscordUserInfo(user));

    await dbService.AddBlacklistAsync(user.Id, ctx.Guild.Id, reason);

    var embedLog = ModmailEmbedBuilder.ToLog.BlacklistAdded(ctx.Guild, ctx.User, user, reason);
    await logChannel.SendMessageAsync(embedLog);
    var builderLog = new DiscordWebhookBuilder().AddEmbed(embedLog);
    await ctx.EditResponseAsync(builderLog);


    if (notifyUser) {
      var member = await ctx.Guild.GetMemberAsync(user.Id);
      var dmEmbed = ModmailEmbedBuilder.ToUser.Blacklisted(ctx.Guild, ctx.User, reason);
      await member.SendMessageAsync(dmEmbed);
    }
  }

  [SlashCommand("remove", "Remove a user from the blacklist.")]
  public async Task Remove(InteractionContext ctx,
                           [Option("user", "The user to remove from the blacklist.")]
                           DiscordUser user
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var option = await dbService.GetOptionAsync(ctx.Guild.Id);
    if (option is null) {
      var embed1 = ModmailEmbedBuilder.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder1 = new DiscordWebhookBuilder().AddEmbed(embed1);
      await ctx.EditResponseAsync(builder1);
      return;
    }

    var logChannelId = option.LogChannelId;
    var logChannel = ctx.Guild.GetChannel(logChannelId);

    if (logChannel is null) {
      var embed2 = ModmailEmbedBuilder.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.EditResponseAsync(builder2);
      return;
    }

    var isBlocked = await dbService.GetUserBlacklistStatus(user.Id);
    if (!isBlocked) {
      var embed4 = ModmailEmbedBuilder.Base("User is not blacklisted!", "", DiscordColor.Yellow);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.EditResponseAsync(builder);
      return;
    }

    await dbService.RemoveBlacklistAsync(user.Id);
    var embedLog = ModmailEmbedBuilder.ToLog.BlacklistRemoved(ctx.Guild, ctx.User, user);
    await logChannel.SendMessageAsync(embedLog);
    var builderLog = new DiscordWebhookBuilder().AddEmbed(embedLog);
    await ctx.EditResponseAsync(builderLog);
  }

  [SlashCommand("status", "Check if a user is blacklisted.")]
  public async Task Status(InteractionContext ctx,
                           [Option("user", "The user to check.")] DiscordUser user
  ) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var isBlocked = await dbService.GetUserBlacklistStatus(user.Id);
    var embed = ModmailEmbedBuilder.Base("User Blacklist Status",
                                         isBlocked
                                           ? "User is blacklisted."
                                           : "User is not blacklisted.",
                                         isBlocked
                                           ? DiscordColor.Red
                                           : DiscordColor.Green);
    var builder = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.EditResponseAsync(builder);
  }

  [SlashCommand("view", "View all blacklisted users.")]
  public async Task View(InteractionContext ctx) {
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();
    var blacklistedUsers = (await dbService.GetBlacklistedUsersAsync(ctx.Guild.Id)).Select(x => $"<@{x}>");
    var embed = ModmailEmbedBuilder.Base("Blacklisted Users", string.Join("\n", blacklistedUsers), DiscordColor.Green);
    var builder = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.EditResponseAsync(builder);
  }
}