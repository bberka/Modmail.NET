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

    if (!string.IsNullOrEmpty(guildOption.GreetingMessage))
      embed.AddField(LangKeys.GREETING_MESSAGE.GetTranslation(), guildOption.GreetingMessage);

    if (!string.IsNullOrEmpty(guildOption.ClosingMessage))
      embed.AddField(LangKeys.CLOSING_MESSAGE.GetTranslation(), guildOption.ClosingMessage);

    return new DiscordWebhookBuilder().AddEmbed(embed);
  }

  public static DiscordWebhookBuilder ListTeams(DiscordGuild guild, List<GuildTeam> teams) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_LIST.GetTranslation())
                .WithColor(Colors.InfoColor);

    foreach (var team in teams) {
      var sb = new StringBuilder();
      sb.AppendLine($"`{LangKeys.ENABLED}`: {team.IsEnabled}");
      sb.AppendLine($"`{LangKeys.PERMISSION_LEVEL}`: {team.PermissionLevel}");
      sb.AppendLine($"`{LangKeys.MEMBERS}`: {team.GuildTeamMembers.Count}");
      sb.AppendLine($"`{LangKeys.PING_ON_NEW_TICKET}`: {team.PingOnNewTicket}");
      sb.AppendLine($"`{LangKeys.PING_ON_NEW_MESSAGE}`: {team.PingOnNewMessage}");
      foreach (var member in team.GuildTeamMembers.OrderBy(x => x.Type))
        switch (member.Type) {
          case TeamMemberDataType.RoleId:
            //Tag role 
            sb.AppendLine($"`{LangKeys.ROLE}`: <@&{member.Key}>");
            break;
          case TeamMemberDataType.UserId:
            //tag member
            sb.AppendLine($"`{LangKeys.USER}`: <@{member.Key}>");
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }

      embed.AddField(team.Name, sb.ToString());
    }

    return new DiscordWebhookBuilder().AddEmbed(embed);
  }
}