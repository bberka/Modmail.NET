using DSharpPlus;
using DSharpPlus.Entities;

namespace Modmail.NET.Common;

public static class ModmailInteractions
{
  /// <summary>
  ///   Creates interaction response builder for taking feedback about your server's team moderation.
  /// </summary>
  /// <returns></returns>
  public static DiscordMessageBuilder CreateFeedbackInteraction(Guid ticketId, DiscordGuild guild) {
    var builder = new DiscordMessageBuilder();
    var starList = new List<DiscordComponent> {
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, ticketId), "1", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, ticketId), "2", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, ticketId), "3", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, ticketId), "4", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, ticketId), "5", false, new DiscordComponentEmoji("⭐"))
    };

    var embed = ModmailEmbeds.Interaction.EmbedFeedback(guild);
    var response = builder
                   .AddEmbed(embed)
                   .AddComponents(starList);
    return response;
  }

  public static DiscordInteractionResponseBuilder CreateFeedbackModal(int starCount, Guid ticketId, ulong messageId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Feedback")
                .WithCustomId(UtilInteraction.BuildKey("feedback", starCount, ticketId, messageId))
                .AddComponents(new TextInputComponent("Feedback",
                                                      "feedback",
                                                      "Please tell us reasons for your rating",
                                                      style: TextInputStyle.Paragraph,
                                                      min_length: 10,
                                                      max_length: 500));
    return modal;
  }
}