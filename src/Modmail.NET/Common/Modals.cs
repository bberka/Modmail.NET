using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

public static class Modals
{
  public static DiscordInteractionResponseBuilder CreateFeedbackModal(int starCount, Guid ticketId, ulong messageId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(Texts.FEEDBACK)
                .WithCustomId(UtilInteraction.BuildKey("feedback", starCount, ticketId, messageId))
                .AddComponents(new TextInputComponent(Texts.FEEDBACK,
                                                      "feedback",
                                                      Texts.PLEASE_TELL_US_REASONS_FOR_YOUR_RATING,
                                                      style: TextInputStyle.Paragraph,
                                                      min_length: 10,
                                                      max_length: 500));
    return modal;
  }

  public static DiscordInteractionResponseBuilder CreateCloseTicketWithReasonModal(Guid ticketId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(Texts.CLOSE_TICKET_WITH_REASON)
                .WithCustomId(UtilInteraction.BuildKey("close_ticket_with_reason", ticketId))
                .AddComponents(new TextInputComponent(Texts.REASON,
                                                      "reason",
                                                      Texts.ENTER_A_REASON_FOR_CLOSING_THIS_TICKET,
                                                      style: TextInputStyle.Paragraph,
                                                      min_length: 10,
                                                      max_length: 500));
    return modal;
  }
}