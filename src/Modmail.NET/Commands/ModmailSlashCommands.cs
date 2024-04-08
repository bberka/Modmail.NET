using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Aspects;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Serilog;

namespace Modmail.NET.Commands;

[PerformanceLoggerAspect(ThresholdMs = 3000)]
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
                          string? closingMessage = null,
                          [Option("ticket-timeout-hours", "The number of hours before a ticket is automatically closed. Default is 72 hours.")]
                          int ticketTimeoutHours = Const.DEFAULT_TICKET_TIMEOUT_HOURS
  ) {
    const string logMessage = $"[{nameof(ModmailSlashCommands)}]{nameof(Setup)}({{ContextUserId}},{{sensitiveLogging}},{{TakeFeedbackAfterClosing}},{{GreetingMessage}},{{ClosingMessage}})";

    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    try {
      await GuildOption.ProcessSetupAsync(ctx.Guild,
                                          sensitiveLogging,
                                          takeFeedbackAfterClosing,
                                          greetingMessage,
                                          closingMessage,
                                          ticketTimeoutHours);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(LangKeys.SERVER_SETUP_COMPLETE.GetTranslation()));
      Log.Information(logMessage,
                      ctx.User.Id,
                      sensitiveLogging,
                      takeFeedbackAfterClosing,
                      greetingMessage,
                      closingMessage);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  sensitiveLogging,
                  takeFeedbackAfterClosing,
                  greetingMessage,
                  closingMessage);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                sensitiveLogging,
                takeFeedbackAfterClosing,
                greetingMessage,
                closingMessage);
    }
  }

  [SlashCommand("configure", "Configure the modmail bot.")]
  public async Task Configure(InteractionContext ctx,
                              [Option("sensitive-logging", "Whether to log modmail messages")]
                              bool? sensitiveLogging = null,
                              [Option("take-feedback", "Whether to take feedback after closing tickets")]
                              bool? takeFeedbackAfterClosing = null,
                              [Option("greeting-message", "The greeting message")]
                              string? greetingMessage = null,
                              [Option("closing-message", "The closing message")]
                              string? closingMessage = null,
                              [Option("ticket-timeout-hours", "The number of hours before a ticket is automatically closed.")]
                              int? ticketTimeoutHours = null
  ) {
    const string logMessage = $"[{nameof(ModmailSlashCommands)}]{nameof(Configure)}({{ContextUserId}},{{sensitiveLogging}},{{TakeFeedbackAfterClosing}},{{GreetingMessage}},{{ClosingMessage}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var guildOption = await GuildOption.GetAsync();
      await guildOption.ProcessConfigureAsync(ctx.Guild,
                                              sensitiveLogging,
                                              takeFeedbackAfterClosing,
                                              greetingMessage,
                                              closingMessage,
                                              ticketTimeoutHours);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(LangKeys.SERVER_CONFIG_UPDATED.GetTranslation()));
      Log.Information(logMessage,
                      ctx.User.Id,
                      sensitiveLogging,
                      takeFeedbackAfterClosing,
                      greetingMessage,
                      closingMessage);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  sensitiveLogging,
                  takeFeedbackAfterClosing,
                  greetingMessage,
                  closingMessage);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                sensitiveLogging,
                takeFeedbackAfterClosing,
                greetingMessage,
                closingMessage);
    }
  }


  [SlashCommand("get-settings", "Get the modmail bot settings.")]
  public async Task GetSettings(InteractionContext ctx) {
    const string logMessage = $"[{nameof(ModmailSlashCommands)}]{nameof(GetSettings)}({{ContextUserId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
    try {
      var guildOption = await GuildOption.GetAsync();
      await ctx.Interaction.EditOriginalResponseAsync(CommandResponses.Settings(guildOption));
      Log.Information(logMessage, ctx.User.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id);
    }
  }
}