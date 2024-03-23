using DSharpPlus;
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
      var embed1 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder1 = new DiscordWebhookBuilder().AddEmbed(embed1);
      await ctx.EditResponseAsync(builder1);
      return;
    }

    var logChannelId = option.LogChannelId;
    var logChannel = ctx.Guild.GetChannel(logChannelId);

    if (logChannel is null) {
      var embed2 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.EditResponseAsync(builder2);
      return;
    }

    var activeTicket = await dbService.GetActiveTicketAsync(user.Id);
    if (activeTicket is not null) {
      var embed3 = ModmailEmbeds.Base(Texts.USER_HAS_ACTIVE_TICKET, Texts.PLEASE_CLOSE_THE_TICKET_BEFORE_BLACKLISTING, DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.EditResponseAsync(builder);
      return;
    }


    var activeBlock = await dbService.GetUserBlacklistStatus(user.Id);
    if (activeBlock) {
      var embed4 = ModmailEmbeds.Base(Texts.USER_ALREADY_BLACKLISTED, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.EditResponseAsync(builder);
      return;
    }

    await dbService.UpdateUserInfoAsync(new DiscordUserInfo(ctx.User));
    await dbService.UpdateUserInfoAsync(new DiscordUserInfo(user));

    await dbService.AddBlacklistAsync(user.Id, ctx.Guild.Id, reason);

    var embedLog = ModmailEmbeds.ToLog.BlacklistAdded(ctx.User, user, reason);
    await logChannel.SendMessageAsync(embedLog);
    var builderLog = new DiscordWebhookBuilder().AddEmbed(embedLog);
    await ctx.EditResponseAsync(builderLog);


    if (notifyUser) {
      var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
      if (member is null) {
        var embed5 = ModmailEmbeds.Base(Texts.USER_NOT_FOUND, "", DiscordColor.Red);
        var builder = new DiscordWebhookBuilder().AddEmbed(embed5);
        await ctx.EditResponseAsync(builder);
        return;
      }

      var dmEmbed = ModmailEmbeds.ToUser.Blacklisted(ctx.Guild, ctx.User, reason);
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
      var embed1 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder1 = new DiscordWebhookBuilder().AddEmbed(embed1);
      await ctx.EditResponseAsync(builder1);
      return;
    }

    var logChannelId = option.LogChannelId;
    var logChannel = ctx.Guild.GetChannel(logChannelId);

    if (logChannel is null) {
      var embed2 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, Texts.SETUP_SERVER_BEFORE_USING, DiscordColor.Red);
      var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.EditResponseAsync(builder2);
      return;
    }

    var isBlocked = await dbService.GetUserBlacklistStatus(user.Id);
    if (!isBlocked) {
      var embed4 = ModmailEmbeds.Base(Texts.USER_IS_NOT_BLACKLISTED, "", DiscordColor.Yellow);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed4);
      await ctx.EditResponseAsync(builder);
      return;
    }

    await dbService.RemoveBlacklistAsync(user.Id);
    var embedLog = ModmailEmbeds.ToLog.BlacklistRemoved(ctx.User, user);
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
    var embed = ModmailEmbeds.Base(Texts.USER_BLACKLIST_STATUS,
                                   isBlocked
                                     ? Texts.USER_IS_BLACKLISTED
                                     : Texts.USER_IS_NOT_BLACKLISTED,
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
    var embed = ModmailEmbeds.Base(Texts.BLACKLISTED_USERS, string.Join("\n", blacklistedUsers), DiscordColor.Green);
    var builder = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.EditResponseAsync(builder);
  }
}