using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static class EmbedUser
{
  public static DiscordEmbed FeedbackReceivedUpdateMessage(Ticket ticket) {
    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(Texts.FEEDBACK_RECEIVED)
                       .WithCustomTimestamp()
                       .WithGuildInfoFooter(ticket.GuildOption)
                       .AddField(Texts.STAR, Texts.STAR_EMOJI + ticket.FeedbackStar)
                       .AddField(Texts.FEEDBACK, ticket.FeedbackMessage)
                       .WithColor(Colors.FeedbackColor);
    return feedbackDone;
  }
}