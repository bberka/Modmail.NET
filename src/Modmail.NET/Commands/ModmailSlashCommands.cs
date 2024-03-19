using System.Text;
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
                          bool sensitiveLogging = true,
                          [Option("take-feedback", "Whether to take feedback after closing tickets")]
                          bool takeFeedbackAfterClosing = false
    // ,
    // [Option("show-confirmation", "Whether to show confirmation when closing tickets")]
    // bool showConfirmationWhenClosing = false
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
      RegisterDate = DateTime.Now,
      TakeFeedbackAfterClosing = takeFeedbackAfterClosing,
      ShowConfirmationWhenClosingTickets = false
      // AllowAnonymousResponding = allowAnonymousResponding,
    };
    await dbService.AddGuildOptionAsync(guildOption);

    var embed2 = ModmailEmbedBuilder.Base("Server setup complete!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed2);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
    Log.Information("Server setup complete for guild: {GuildOptionId}", currentGuildId);
  }


  [SlashCommand("get-settings", "Get the modmail bot settings.")]
  public async Task GetSettings(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var guildOption = await dbService.GetOptionAsync(currentGuildId);
    if (guildOption is null) {
      var embed3 = ModmailEmbedBuilder.Base("Server not setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbedBuilder.Settings(ctx.Guild, guildOption);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }


  [SlashCommand("toggle-sensitive-logging", "Toggle sensitive logging for the modmail bot.")]
  public async Task ToggleSensitiveLogging(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var ticketOption = await dbService.GetOptionAsync(currentGuildId);
    if (ticketOption is null) {
      var embed3 = ModmailEmbedBuilder.Base("Server not setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    ticketOption.IsSensitiveLogging = !ticketOption.IsSensitiveLogging;
    await dbService.UpdateGuildOptionAsync(ticketOption);

    var text = new StringBuilder();
    if (ticketOption.IsSensitiveLogging) {
      text.Append("Sensitive logging enabled!");
      Log.Information("Sensitive logging enabled for guild: {GuildOptionId}", currentGuildId);
    }
    else {
      text.Append("Sensitive logging disabled!");
      Log.Information("Sensitive logging disabled for guild: {GuildOptionId}", currentGuildId);
    }

    var embed4 = ModmailEmbedBuilder.Base(text.ToString(), "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed4);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("toggle-take-feedback", "Toggle taking feedback after closing tickets.")]
  public async Task ToggleTakeFeedback(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var dbService = ServiceLocator.Get<IDbService>();

    var currentGuildId = ctx.Guild.Id;
    var ticketOption = await dbService.GetOptionAsync(currentGuildId);
    if (ticketOption is null) {
      var embed3 = ModmailEmbedBuilder.Base("Server not setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    ticketOption.TakeFeedbackAfterClosing = !ticketOption.TakeFeedbackAfterClosing;
    await dbService.UpdateGuildOptionAsync(ticketOption);

    var text = new StringBuilder();
    if (ticketOption.TakeFeedbackAfterClosing) {
      text.Append("Taking feedback after closing tickets enabled!");
      Log.Information("Taking feedback after closing tickets enabled for guild: {GuildOptionId}", currentGuildId);
    }
    else {
      text.Append("Taking feedback after closing tickets disabled!");
      Log.Information("Taking feedback after closing tickets disabled for guild: {GuildOptionId}", currentGuildId);
    }

    var embed4 = ModmailEmbedBuilder.Base(text.ToString(), "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed4);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
  
  [SlashCommand("set-greeting-message", "Set the greeting message for the modmail bot.")]
  public async Task SetGreetingMessage(InteractionContext ctx,
                                      [Option("message", "The greeting message")]
                                      string message) {
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

    guildOption.GreetingMessage = message;
    await dbService.UpdateGuildOptionAsync(guildOption);

    var embed4 = ModmailEmbedBuilder.Base("Greeting message updated!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed4);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
  
  [SlashCommand("set-closing-message", "Set the closing message for the modmail bot.")]
  public async Task SetClosingMessage(InteractionContext ctx,
                                      [Option("message", "The closing message")]
                                      string message) {
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

    guildOption.ClosingMessage = message;
    await dbService.UpdateGuildOptionAsync(guildOption);

    var embed4 = ModmailEmbedBuilder.Base("Closing message updated!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed4);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
  

}