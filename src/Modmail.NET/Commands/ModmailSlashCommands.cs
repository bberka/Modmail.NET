using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("modmail", "Modmail management commands.")]
[RequireAdmin]

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

    var mainGuild = ctx.Guild;
    var category = await mainGuild.CreateChannelCategoryAsync(Const.CATEGORY_NAME);
    var logChannel = await mainGuild.CreateTextChannelAsync(Const.LOG_CHANNEL_NAME, category);
    var categoryId = category.Id;
   var  logChannelId = logChannel.Id;
    var guildOption = new GuildOption {
      CategoryId = categoryId,
      GuildId = mainGuild.Id,
      LogChannelId = logChannelId,
      IsSensitiveLogging = sensitiveLogging,
      IsEnabled = true,
      RegisterDate = DateTime.Now,
      TakeFeedbackAfterClosing = takeFeedbackAfterClosing,
      ShowConfirmationWhenClosingTickets = false,
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
    var ticketOption = await dbService.GetOptionAsync(currentGuildId);
    if (ticketOption is null) {
      var embed3 = ModmailEmbedBuilder.Base("Server not setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbedBuilder.Settings(ctx.Guild, ticketOption);
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
    await dbService.UpdateTicketOptionAsync(ticketOption);

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
    await dbService.UpdateTicketOptionAsync(ticketOption);

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
   
  
}