using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static class CommandResponses
{
  public static DiscordWebhookBuilder Settings(GuildOption guildOption) {
    var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithCustomTimestamp()
                .WithTitle(Texts.MODMAIL_SETTINGS)
                .WithColor(Colors.InfoColor)
                .WithGuildInfoFooter(guildOption);

    sb.AppendLine($"`{Texts.ENABLED}`: " + guildOption.IsEnabled);
    sb.AppendLine($"`{Texts.SENSITIVE_LOGGING}`: " + guildOption.IsSensitiveLogging);
    sb.AppendLine($"`{Texts.TAKE_FEEDBACK_AFTER_CLOSING}`: " + guildOption.TakeFeedbackAfterClosing);
    sb.AppendLine($"`{Texts.SHOW_CONFIRMATIONS}`: " + guildOption.ShowConfirmationWhenClosingTickets);
    // sb.AppendLine("`Allow Anonymous Response`: " + guildOption.AllowAnonymousResponding);
    sb.AppendLine($"`{Texts.LOG_CHANNEL}`: <#" + guildOption.LogChannelId + "> | " + guildOption.LogChannelId);
    sb.AppendLine($"`{Texts.TICKET_CATEGORY}`: <#" + guildOption.CategoryId + "> | " + guildOption.CategoryId);

    embed.WithDescription(sb.ToString());

    if (!string.IsNullOrEmpty(guildOption.GreetingMessage))
      embed.AddField(Texts.GREETING_MESSAGE, guildOption.GreetingMessage);

    if (!string.IsNullOrEmpty(guildOption.ClosingMessage))
      embed.AddField(Texts.CLOSING_MESSAGE, guildOption.ClosingMessage);

    return new DiscordWebhookBuilder().AddEmbed(embed);
  }

  public static DiscordWebhookBuilder ListTeams(DiscordGuild guild, List<GuildTeam> teams) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_LIST)
                .WithColor(Colors.InfoColor);

    foreach (var team in teams) {
      var sb = new StringBuilder();
      sb.AppendLine($"`{Texts.ENABLED}`: {team.IsEnabled}");
      sb.AppendLine($"`{Texts.PERMISSION_LEVEL}`: {team.PermissionLevel}");
      sb.AppendLine($"`{Texts.MEMBERS}`: {team.GuildTeamMembers.Count}");
      sb.AppendLine($"`{Texts.PING_ON_NEW_TICKET}`: {team.PingOnNewTicket}");
      sb.AppendLine($"`{Texts.PING_ON_NEW_MESSAGE}`: {team.PingOnNewMessage}");
      foreach (var member in team.GuildTeamMembers.OrderBy(x => x.Type))
        switch (member.Type) {
          case TeamMemberDataType.RoleId:
            //Tag role 
            sb.AppendLine($"`{Texts.ROLE}`: <@&{member.Key}>");
            break;
          case TeamMemberDataType.UserId:
            //tag member
            sb.AppendLine($"`{Texts.USER}`: <@{member.Key}>");
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }

      embed.AddField(team.Name, sb.ToString());
    }

    return new DiscordWebhookBuilder().AddEmbed(embed);
  }
}