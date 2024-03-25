using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

public static class Modals
{
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