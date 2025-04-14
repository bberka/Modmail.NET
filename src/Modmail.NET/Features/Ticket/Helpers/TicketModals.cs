using DSharpPlus.Entities;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Helpers;

public static class TicketModals
{
  public static DiscordInteractionResponseBuilder CreateFeedbackModal(int starCount, Guid ticketId, ulong messageId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(Lang.Feedback.Translate())
                .WithCustomId(UtilInteraction.BuildKey("feedback", starCount, ticketId, messageId))
                .AddComponents(new DiscordTextInputComponent(Lang.Feedback.Translate(),
                                                             "feedback",
                                                             Lang.PleaseTellUsReasonsForYourRating.Translate(),
                                                             style: DiscordTextInputStyle.Paragraph,
                                                             required: false,
                                                             max_length: DbLength.FeedbackMessage));
    return modal;
  }

  public static DiscordInteractionResponseBuilder CreateCloseTicketWithReasonModal(Guid ticketId) {
    var modal = new DiscordInteractionResponseBuilder()
                .WithTitle(Lang.CloseTicket.Translate())
                .WithCustomId(UtilInteraction.BuildKey("close_ticket_with_reason", ticketId))
                .AddComponents(new DiscordTextInputComponent(Lang.Reason.Translate(),
                                                             "reason",
                                                             Lang.EnterReasonForClosingThisTicket.Translate(),
                                                             style: DiscordTextInputStyle.Paragraph,
                                                             required: false,
                                                             max_length: DbLength.Reason));
    return modal;
  }
}