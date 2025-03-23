using DSharpPlus.Entities;
using Modmail.NET.Utils;

namespace Modmail.NET.Static;

public static class Modals
{
  public static DiscordInteractionResponseBuilder CreateFeedbackModal(int starCount, Guid ticketId, ulong messageId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(LangKeys.FEEDBACK.GetTranslation())
                .WithCustomId(UtilInteraction.BuildKey("feedback", starCount, ticketId, messageId))
                .AddComponents(new DiscordTextInputComponent(LangKeys.FEEDBACK.GetTranslation(),
                                                             "feedback",
                                                             LangKeys.PLEASE_TELL_US_REASONS_FOR_YOUR_RATING.GetTranslation(),
                                                             style: DiscordTextInputStyle.Paragraph,
                                                             min_length: 10,
                                                             max_length: 500));
    return modal;
  }

  public static DiscordInteractionResponseBuilder CreateCloseTicketWithReasonModal(Guid ticketId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(LangKeys.CLOSE_TICKET_WITH_REASON.GetTranslation())
                .WithCustomId(UtilInteraction.BuildKey("close_ticket_with_reason", ticketId))
                .AddComponents(new DiscordTextInputComponent(LangKeys.REASON.GetTranslation(),
                                                             "reason",
                                                             LangKeys.ENTER_A_REASON_FOR_CLOSING_THIS_TICKET.GetTranslation(),
                                                             style: DiscordTextInputStyle.Paragraph,
                                                             min_length: 10,
                                                             max_length: 500));
    return modal;
  }
}