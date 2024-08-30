using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;

namespace Modmail.NET.Common;

public static class CommandResponses
{
  public static DiscordWebhookBuilder Settings(GuildOption guildOption) {
    var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithCustomTimestamp()
                .WithTitle(LangKeys.MODMAIL_SETTINGS.GetTranslation())
                .WithColor(Colors.InfoColor)
                .WithGuildInfoFooter(guildOption);

    sb.AppendLine($"`{LangKeys.ENABLED.GetTranslation()}`: " + guildOption.IsEnabled);
    sb.AppendLine($"`{LangKeys.SENSITIVE_LOGGING.GetTranslation()}`: " + guildOption.IsSensitiveLogging);
    sb.AppendLine($"`{LangKeys.TAKE_FEEDBACK_AFTER_CLOSING.GetTranslation()}`: " + guildOption.TakeFeedbackAfterClosing);
    sb.AppendLine($"`{LangKeys.SHOW_CONFIRMATIONS.GetTranslation()}`: " + guildOption.ShowConfirmationWhenClosingTickets);
    // sb.AppendLine("`Allow Anonymous Response`: " + guildOption.AllowAnonymousResponding);
    sb.AppendLine($"`{LangKeys.LOG_CHANNEL.GetTranslation()}`: <#" + guildOption.LogChannelId + "> | " + guildOption.LogChannelId);
    sb.AppendLine($"`{LangKeys.TICKET_CATEGORY.GetTranslation()}`: <#" + guildOption.CategoryId + "> | " + guildOption.CategoryId);

    embed.WithDescription(sb.ToString());

    return new DiscordWebhookBuilder().AddEmbed(embed);
  }
}