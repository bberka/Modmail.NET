using DSharpPlus.Entities;
using Modmail.NET.Common.Utils;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Helpers;

public static class TicketModals
{
  public static DiscordInteractionResponseBuilder CreateFeedbackModal(int starCount, Guid ticketId, ulong messageId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(LangKeys.Feedback.GetTranslation())
                .WithCustomId(UtilInteraction.BuildKey("feedback", starCount, ticketId, messageId))
                .AddComponents(new DiscordTextInputComponent(LangKeys.Feedback.GetTranslation(),
                                                             "feedback",
                                                             LangKeys.PleaseTellUsReasonsForYourRating.GetTranslation(),
                                                             style: DiscordTextInputStyle.Paragraph,
                                                             min_length: 10,
                                                             max_length: 500));
    return modal;
  }

  public static DiscordInteractionResponseBuilder CreateCloseTicketWithReasonModal(Guid ticketId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(LangKeys.CloseTicket.GetTranslation())
                .WithCustomId(UtilInteraction.BuildKey("close_ticket_with_reason", ticketId))
                .AddComponents(new DiscordTextInputComponent(LangKeys.Reason.GetTranslation(),
                                                             "reason",
                                                             LangKeys.EnterReasonForClosingThisTicket.GetTranslation(),
                                                             style: DiscordTextInputStyle.Paragraph,
                                                             required: false,
                                                             max_length: 500));
    return modal;
  }
}