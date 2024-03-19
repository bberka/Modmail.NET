using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("modmail", "Modmail management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
public class ModmailSlashCommands : ApplicationCommandModule
{
  [SlashCommand("setup", "Setup the modmail bot.")]
  public async Task Setup(InteractionContext ctx,
                          [Option("sensitive-logging", "Whether to log modmail messages")]
                          bool sensitiveLogging = false,
                          [Option("take-feedback", "Whether to take feedback after closing tickets")]
                          bool takeFeedbackAfterClosing = false,
                          [Option("greening-message", "The greeting message")]
                          string? greetingMessage = null,
                          [Option("closing-message", "The closing message")]
                          string? closingMessage = null
  ) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbedBuilder.Base("This command can only be used in the main server.", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    // await using var db = new ModmailDbContext();
    var existingMmOption = await dbService.GetOptionAsync(currentGuildId);
    if (existingMmOption is not null) {
      var embed3 = ModmailEmbedBuilder.Base("Server already setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var guild = ctx.Guild;
    var guildId = guild.Id;

    var permissions = await dbService.GetPermissionInfoOrHigherAsync(guildId, TeamPermissionLevel.Admin);
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
    var logChannel = await guild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category, "Modmail log channel", permissionOverwrites);
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
    await dbService.AddGuildOptionAsync(guildOption);

    var embed2 = ModmailEmbedBuilder.Base("Server setup complete!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
    Log.Information("Server setup complete for guild: {GuildOptionId}", currentGuildId);
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

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var guildOption = await dbService.GetOptionAsync(currentGuildId);
    if (guildOption is null) {
      var embed3 = ModmailEmbedBuilder.Base("Server not setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
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
    await dbService.UpdateGuildOptionAsync(guildOption);

    var embed2 = ModmailEmbedBuilder.Base("Server configuration updated!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
    Log.Information("Server configuration updated for guild: {GuildOptionId}", currentGuildId);
  }

  [SlashCommand("get-settings", "Get the modmail bot settings.")]
  public async Task GetSettings(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var guildOption = await dbService.GetOptionAsync(currentGuildId);
    if (guildOption is null) {
      var embed3 = ModmailEmbedBuilder.ErrorServerNotSetup();
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbedBuilder.Settings(ctx.Guild, guildOption);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
}