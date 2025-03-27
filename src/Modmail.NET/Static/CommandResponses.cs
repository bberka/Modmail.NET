using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;

namespace Modmail.NET.Static;

public static class CommandResponses
{
  public static DiscordWebhookBuilder Settings(GuildOption guildOption) {
    var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithCustomTimestamp()
                .WithTitle(LangKeys.ModmailSettings.GetTranslation())
                .WithColor(Colors.InfoColor)
                .WithGuildInfoFooter(guildOption);

    sb.AppendLine($"`{LangKeys.Enabled.GetTranslation()}`: " + guildOption.IsEnabled);
    sb.AppendLine($"`{LangKeys.TakeFeedbackAfterClosing.GetTranslation()}`: " + guildOption.TakeFeedbackAfterClosing);
    sb.AppendLine($"`{LangKeys.ShowConfirmations.GetTranslation()}`: " + guildOption.ShowConfirmationWhenClosingTickets);
    // sb.AppendLine("`Allow Anonymous Response`: " + guildOption.AllowAnonymousResponding);
    sb.AppendLine($"`{LangKeys.LogChannel.GetTranslation()}`: <#" + guildOption.LogChannelId + "> | " + guildOption.LogChannelId);
    sb.AppendLine($"`{LangKeys.TicketCategory.GetTranslation()}`: <#" + guildOption.CategoryId + "> | " + guildOption.CategoryId);

    embed.WithDescription(sb.ToString());

    return new DiscordWebhookBuilder().AddEmbed(embed);
  }
}