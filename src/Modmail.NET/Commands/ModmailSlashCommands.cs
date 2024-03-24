using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("modmail", "Modmail management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
[UpdateUserInformation]
[RequireMainServer]
public class ModmailSlashCommands : ApplicationCommandModule
{
  [SlashCommand("setup", "Setup the modmail bot.")]
  public async Task Setup(InteractionContext ctx,
                          [Option("sensitive-logging", "Whether to log modmail messages")]
                          bool sensitiveLogging = false,
                          [Option("take-feedback", "Whether to take feedback after closing tickets")]
                          bool takeFeedbackAfterClosing = false,
                          [Option("greeting-message", "The greeting message")]
                          string? greetingMessage = null,
                          [Option("closing-message", "The closing message")]
                          string? closingMessage = null
  ) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    // await using var db = new ModmailDbContext();
    var existingMmOption = await GuildOption.GetAsync();
    if (existingMmOption is not null) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_SERVER_ALREADY_SETUP, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var anyServerSetup = await GuildOption.Any();
    if (anyServerSetup) {
      var embed3 = ModmailEmbeds.Base(Texts.ANOTHER_SERVER_ALREADY_SETUP, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var guild = ctx.Guild;
    var guildId = guild.Id;

    var permissions = await GuildTeamMember.GetPermissionInfoOrHigherAsync(guildId, TeamPermissionLevel.Admin);
    var members = await guild.GetAllMembersAsync();
    var roles = guild.Roles;

    var roleListForOverwrites = new List<DiscordRole>();
    var memberListForOverwrites = new List<DiscordMember>();
    foreach (var perm in permissions) {
      var role = roles.FirstOrDefault(x => x.Key == perm.Key && perm.Type == TeamMemberDataType.RoleId);
      if (role.Key != 0) roleListForOverwrites.Add(role.Value);
      var member2 = members.FirstOrDefault(x => x.Id == perm.Key && perm.Type == TeamMemberDataType.UserId);
      if (member2 is not null && member2.Id != 0) memberListForOverwrites.Add(member2);
    }


    var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, memberListForOverwrites, roleListForOverwrites);


    var category = await guild.CreateChannelCategoryAsync(Const.CATEGORY_NAME, permissionOverwrites);
    var logChannel = await guild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category, Texts.MODMAIL_LOG_CHANNEL_TOPIC, permissionOverwrites);
    var categoryId = category.Id;
    var logChannelId = logChannel.Id;
    var guildOption = new GuildOption {
      CategoryId = categoryId,
      GuildId = guild.Id,
      LogChannelId = logChannelId,
      IsSensitiveLogging = sensitiveLogging,
      IsEnabled = true,
      RegisterDateUtc = DateTime.UtcNow,
      TakeFeedbackAfterClosing = takeFeedbackAfterClosing,
      ShowConfirmationWhenClosingTickets = false
      // AllowAnonymousResponding = allowAnonymousResponding,
    };
    if (!string.IsNullOrEmpty(greetingMessage))
      guildOption.GreetingMessage = greetingMessage;
    if (!string.IsNullOrEmpty(closingMessage))
      guildOption.ClosingMessage = closingMessage;
    await guildOption.AddAsync();

    var embed2 = ModmailEmbeds.Base(Texts.SERVER_SETUP_COMPLETE, "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
    Log.Information("Server setup complete for guild: {GuildId}", currentGuildId);
  }

  [SlashCommand("configure", "Configure the modmail bot.")]
  public async Task Configure(InteractionContext ctx,
                              [Option("sensitive-logging", "Whether to log modmail messages")]
                              bool? sensitiveLogging = null,
                              [Option("take-feedback", "Whether to take feedback after closing tickets")]
                              bool? takeFeedbackAfterClosing = null,
                              [Option("greening-message", "The greeting message")]
                              string? greetingMessage = null,
                              [Option("closing-message", "The closing message")]
                              string? closingMessage = null
  ) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var currentGuildId = ctx.Guild.Id;
    var guildOption = await GuildOption.GetAsync();
    if (guildOption is null) {
      var builder = new DiscordWebhookBuilder().AddEmbed(ModmailEmbeds.ErrorServerNotSetup());
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    guildOption.UpdateDateUtc = DateTime.UtcNow;
    if (sensitiveLogging.HasValue)
      guildOption.IsSensitiveLogging = sensitiveLogging.Value;
    if (takeFeedbackAfterClosing.HasValue)
      guildOption.TakeFeedbackAfterClosing = takeFeedbackAfterClosing.Value;
    if (!string.IsNullOrEmpty(greetingMessage))
      guildOption.GreetingMessage = greetingMessage;
    if (!string.IsNullOrEmpty(closingMessage))
      guildOption.ClosingMessage = closingMessage;
    await guildOption.UpdateAsync();

    var embed2 = ModmailEmbeds.Base(Texts.SERVER_CONFIG_UPDATED, "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
    Log.Information("Server configuration updated for guild: {GuildId}", currentGuildId);
  }

  [SlashCommand("get-settings", "Get the modmail bot settings.")]
  public async Task GetSettings(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());


    var currentGuildId = ctx.Guild.Id;
    var guildOption = await GuildOption.GetAsync();
    if (guildOption is null) {
      var builder = new DiscordWebhookBuilder().AddEmbed(ModmailEmbeds.ErrorServerNotSetup());
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbeds.Settings(ctx.Guild, guildOption);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
}