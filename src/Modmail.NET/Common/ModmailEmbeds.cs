using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static class ModmailEmbeds
{
  public static readonly DiscordColor ErrorColor = DiscordColor.DarkRed;
  public static readonly DiscordColor SuccessColor = DiscordColor.Green;
  public static readonly DiscordColor InfoColor = DiscordColor.CornflowerBlue;

  public static readonly DiscordColor WarningColor = DiscordColor.Orange;
  // private static readonly DiscordColor NeutralInformationColor = DiscordColor.Gold;


  public static readonly DiscordColor MessageReceivedColor = DiscordColor.Blue;
  public static readonly DiscordColor MessageSentColor = DiscordColor.Green;
  public static readonly DiscordColor TicketCreatedColor = DiscordColor.Blue;
  public static readonly DiscordColor TicketClosedColor = DiscordColor.Red;
  public static readonly DiscordColor TicketPriorityChangedColor = DiscordColor.Magenta;
  public static readonly DiscordColor TicketTypeChangedColor = DiscordColor.SpringGreen;
  public static readonly DiscordColor NoteAddedColor = DiscordColor.Teal;
  public static readonly DiscordColor AnonymousToggledColor = DiscordColor.Grayple;
  public static readonly DiscordColor FeedbackColor = DiscordColor.Orange;


  public static DiscordEmbed Base(string title, string text = "", DiscordColor? color = null) {
    color ??= DiscordColor.White;
    var embed = new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithColor(color.Value);
    if (!string.IsNullOrEmpty(text))
      embed.WithDescription(text);
    return embed;
  }

  public static DiscordEmbed ListTeams(DiscordGuild guild, List<GuildTeam> teams) {
    // var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_LIST)
                // .WithDescription(sb.ToString())
                .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                .WithColor(InfoColor);

    foreach (var team in teams) {
      var sb = new StringBuilder();
      sb.AppendLine($"`{Texts.ENABLED}`: {team.IsEnabled}");
      sb.AppendLine($"`{Texts.PERMISSION_LEVEL}`: {team.PermissionLevel}");
      sb.AppendLine($"`{Texts.MEMBERS}`: {team.GuildTeamMembers.Count}");
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

    return embed;
  }

  public static DiscordEmbed Settings(DiscordGuild guild, GuildOption ticketOption) {
    var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithTimestamp(DateTime.Now)
                .WithTitle(Texts.MODMAIL_SETTINGS)
                .WithColor(InfoColor)
                .WithFooter(guild.Name, guild.IconUrl);

    sb.AppendLine($"`{Texts.ENABLED}`: " + ticketOption.IsEnabled);
    sb.AppendLine($"`{Texts.SENSITIVE_LOGGING}`: " + ticketOption.IsSensitiveLogging);
    sb.AppendLine($"`{Texts.TAKE_FEEDBACK_AFTER_CLOSING}`: " + ticketOption.TakeFeedbackAfterClosing);
    sb.AppendLine($"`{Texts.SHOW_CONFIRMATIONS}`: " + ticketOption.ShowConfirmationWhenClosingTickets);
    // sb.AppendLine("`Allow Anonymous Response`: " + ticketOption.AllowAnonymousResponding);
    sb.AppendLine($"`{Texts.LOG_CHANNEL}`: <#" + ticketOption.LogChannelId + "> | " + ticketOption.LogChannelId);
    sb.AppendLine($"`{Texts.TICKET_CATEGORY}`: <#" + ticketOption.CategoryId + "> | " + ticketOption.CategoryId);

    embed.WithDescription(sb.ToString());

    if (!string.IsNullOrEmpty(ticketOption.GreetingMessage))
      embed.AddField(Texts.GREETING_MESSAGE, ticketOption.GreetingMessage);

    if (!string.IsNullOrEmpty(ticketOption.ClosingMessage))
      embed.AddField(Texts.CLOSING_MESSAGE, ticketOption.ClosingMessage);
    return embed;
  }

  public static DiscordEmbed ErrorServerNotSetup() {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.SERVER_NOT_SETUP)
                .WithDescription(Texts.SETUP_SERVER_BEFORE_USING)
                .WithColor(ErrorColor);
    return embed;
  }

  public static class ToUser
  {
    public static DiscordEmbed UserBlocked(string guildName, string guildIcon) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_BEEN_BLACKLISTED)
                  .WithDescription(Texts.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION)
                  .WithFooter(guildName, guildIcon)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(ErrorColor);
      return embed;
    }

    public static DiscordEmbed MessageSent(string guildName,
                                           string guildIcon,
                                           DiscordUser user,
                                           DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  .WithFooter(guildName, guildIcon)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .WithColor(MessageSentColor);
      for (var i = 0; i < message.Attachments.Count; i++)
        embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);
      return embed;
    }

    public static DiscordMessageBuilder TicketCreated(DiscordGuild guild,
                                                      DiscordMessage message,
                                                      GuildOption option,
                                                      List<TicketType> ticketTypes,
                                                      Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_CREATED_NEW_TICKET)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(TicketCreatedColor);
      if (!string.IsNullOrEmpty(option.GreetingMessage))
        embed.WithDescription(option.GreetingMessage);

      var builder = new DiscordMessageBuilder()
        .AddEmbed(embed);

      if (ticketTypes.Count > 0) {
        var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                                                   Texts.PLEASE_SELECT_A_TICKET_TYPE,
                                                   ticketTypes.Select(x => new DiscordSelectComponentOption(x.Name, x.Key.ToString(), x.Description, false, new DiscordComponentEmoji(x.Emoji)))
                                                              .ToList());
        builder.AddComponents(selectBox);
      }

      return builder;
    }

    public static DiscordEmbed TicketCreatedUpdated(DiscordGuild guild,
                                                    GuildOption option,
                                                    TicketType? ticketType) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_CREATED_NEW_TICKET)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTimestamp(DateTime.UtcNow)
                  .WithColor(TicketCreatedColor);

      if (!string.IsNullOrEmpty(option.GreetingMessage))
        embed.WithDescription(option.GreetingMessage);

      if (ticketType is not null) {
        embed.AddField(Texts.TICKET_TYPE, ticketType.Name);
      }

      return embed;
    }


    public static DiscordEmbed MessageReceived(DiscordUser author, DiscordMessage message, DiscordGuild guild, bool anonymous = false) {
      var embed = new DiscordEmbedBuilder()
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(MessageReceivedColor);
      if (!anonymous) embed.WithAuthor(author.Username, iconUrl: author.AvatarUrl);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      return embed;
    }


    public static DiscordEmbed Blacklisted(DiscordGuild guild, DiscordUser user, string? reason) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_BEEN_BLACKLISTED)
                  .WithDescription(Texts.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithColor(ErrorColor);
      if (!string.IsNullOrEmpty(reason)) embed.AddField(Texts.REASON, reason);
      return embed;
    }

    public static DiscordEmbed? TicketTypeEmbedMessage(TicketType ticketType) {
      if (string.IsNullOrEmpty(ticketType.EmbedMessageTitle) || string.IsNullOrEmpty(ticketType.EmbedMessageContent)) return null;
      var embed = new DiscordEmbedBuilder()
        .WithColor(TicketTypeChangedColor);
      if (!string.IsNullOrEmpty(ticketType.EmbedMessageTitle)) embed.WithTitle(ticketType.EmbedMessageTitle);
      if (!string.IsNullOrEmpty(ticketType.EmbedMessageContent)) embed.WithDescription(ticketType.EmbedMessageContent);

      return embed;
    }
  }

  public static class ToMail
  {
    public static DiscordEmbed TicketTypeChanged(DiscordUser user, TicketType type) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.TICKET_TYPE_CHANGED)
                  .WithDescription(string.Format(Texts.TICKET_TYPE_CHANGED_MESSAGE_TO_MAIL, type.Emoji, type.Name))
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(TicketTypeChangedColor);
      return embed;
    }

    public static DiscordMessageBuilder NewTicket(DiscordUser member, Guid ticketId, List<DiscordRole> modRoleListForOverwrites, List<DiscordMember> modMemberListForOverwrites) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NEW_TICKET)
                  .WithTimestamp(DateTime.Now)
                  .WithDescription(Texts.NEW_TICKET_DESCRIPTION_MESSAGE)
                  .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
                  .AddField(Texts.USER, member.Mention, true)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), true)
                  .WithColor(TicketCreatedColor);

      var messageBuilder = new DiscordMessageBuilder()
                           .AddEmbed(embed)
                           .AddComponents(new DiscordButtonComponent(ButtonStyle.Danger,
                                                                     UtilInteraction.BuildKey("close_ticket", ticketId.ToString()),
                                                                     Texts.CLOSE_TICKET,
                                                                     emoji: new DiscordComponentEmoji("🔒"))
                                          // ,
                                          // new DiscordButtonComponent(ButtonStyle.Danger,
                                          //                            UtilInteraction.BuildKey("close_ticket_with_reason", ticketId.ToString()),
                                          //                            Texts.CLOSE_TICKET_WITH_REASON,
                                          //                            emoji: new DiscordComponentEmoji("🔒"))
                                         );

      var sb = new StringBuilder();
      if (modRoleListForOverwrites.Count > 0) {
        sb.AppendLine(Texts.ROLES + ":");
        foreach (var role in modRoleListForOverwrites) sb.AppendLine(role.Mention);
      }

      if (modMemberListForOverwrites.Count > 0) {
        sb.AppendLine(Texts.MEMBERS + ":");
        foreach (var member2 in modMemberListForOverwrites) sb.AppendLine(member2.Mention);
      }

      messageBuilder.WithContent(sb.ToString());

      return messageBuilder;
    }

    public static DiscordEmbed MessageReceived(DiscordUser user,
                                               DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  .WithAuthor(user.GetUsername(), null, user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(MessageReceivedColor);
      for (var i = 0; i < message.Attachments.Count; i++)
        embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);
      return embed;
    }

    public static DiscordEmbed MessageSent(DiscordUser author, DiscordMessage message, bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
                  .WithColor(MessageSentColor);

      for (var i = 0; i < message.Attachments.Count; i++)
        embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      if (ticketAnonymous) embed.WithFooter(Texts.ANONYMOUS_MESSAGE);

      return embed;
    }

    public static DiscordEmbed NoteAdded(DiscordUser user, string note) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NOTE_ADDED)
                  .WithDescription(note)
                  .WithColor(NoteAddedColor)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl);
      return embed;
    }

    public static DiscordEmbed AnonymousToggled(DiscordUser user, bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.ANONYMOUS_TOGGLED)
                  .WithColor(AnonymousToggledColor)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .WithDescription(ticketAnonymous
                                     ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                                     : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);
      return embed;
    }
  }


  public static class ToLog
  {
    public static DiscordEmbed TicketTypeSelected(DiscordUser user, TicketType type, Ticket ticket) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.TICKET_TYPE_CHANGED)
                  .WithDescription(string.Format(Texts.TICKET_TYPE_CHANGED_MESSAGE_TO_MAIL, type.Emoji, type.Name))
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .WithTimestamp(DateTime.Now)
                  .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                  .WithColor(TicketTypeChangedColor);
      return embed;
    }

    public static DiscordEmbed NewTicketCreated(DiscordUser user,
                                                DiscordMessage initialMessage,
                                                DiscordChannel mailChannel,
                                                Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NEW_TICKET_CREATED)
                  .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
                  .WithTimestamp(initialMessage.Timestamp)
                  .WithColor(TicketCreatedColor)
                  .AddField(Texts.USER, user.Mention, true)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), true);
      return embed;
    }

    public static DiscordEmbed MessageSentByMod(DiscordUser mod,
                                                DiscordUserInfo user,
                                                DiscordMessage message,
                                                DiscordChannel channel,
                                                Guid ticketId,
                                                bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.MESSAGE_SENT_BY_MOD)
                  .WithAuthor(mod.GetUsername(), null, mod.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Green)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper())
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.CHANNEL_ID, channel.Id.ToString(), true)
        ;
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      if (ticketAnonymous) embed.AddField(Texts.ANONYMOUS, Texts.THIS_MESSAGE_SENT_ANONYMOUSLY);

      return embed;
    }

    public static DiscordEmbed MessageSentByUser(DiscordUser user,
                                                 DiscordMessage message,
                                                 Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.MESSAGE_SENT_BY_USER)
                  .WithAuthor(user.GetUsername(), null, user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(MessageSentColor)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper())
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
        ;

      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      return embed;
    }

    public static DiscordEmbed NoteAdded(DiscordUser user, string note, Ticket ticket) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NOTE_ADDED)
                  .WithDescription(note)
                  .WithColor(NoteAddedColor)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
        ;
      return embed;
    }

    public static DiscordEmbed AnonymousToggled(DiscordUser user, Ticket ticket, bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.ANONYMOUS_TOGGLED)
                  .WithColor(AnonymousToggledColor)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper());

      embed.WithDescription(anonymous
                              ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                              : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);

      return embed;
    }

    public static DiscordEmbed BlacklistAdded(DiscordUser author, DiscordUser user, string? reason) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.USER_BLACKLISTED)
                  .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
                  .WithColor(InfoColor)
                  .AddField(Texts.USER, user.Mention, true)
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.USERNAME, user.GetUsername(), true);

      return embed;
    }

    public static DiscordEmbed BlacklistRemoved(DiscordUser author, DiscordUser user) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.USER_BLACKLIST_REMOVED)
                  .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
                  .WithColor(InfoColor)
                  .AddField(Texts.USER, user.Mention, true)
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.USERNAME, user.GetUsername(), true);
      return embed;
    }
  }

  public static class Interaction
  {
    public static DiscordInteractionResponseBuilder Error(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(ErrorColor);
      return new DiscordInteractionResponseBuilder().AddEmbed(embed);
    }

    public static DiscordInteractionResponseBuilder Success(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(SuccessColor);
      return new DiscordInteractionResponseBuilder().AddEmbed(embed);
    }

    public static DiscordInteractionResponseBuilder Info(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(InfoColor);
      return new DiscordInteractionResponseBuilder().AddEmbed(embed);
    }

    public static DiscordInteractionResponseBuilder Warning(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(WarningColor);
      return new DiscordInteractionResponseBuilder().AddEmbed(embed);
    }
  }

  public static class Webhook
  {
    public static DiscordWebhookBuilder Error(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(ErrorColor);
      return new DiscordWebhookBuilder().AddEmbed(embed);
    }

    public static DiscordWebhookBuilder Success(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(SuccessColor);
      return new DiscordWebhookBuilder().AddEmbed(embed);
    }

    public static DiscordWebhookBuilder Info(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(InfoColor);
      return new DiscordWebhookBuilder().AddEmbed(embed);
    }

    public static DiscordWebhookBuilder Warning(string title, string message = "") {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(title)
                  .WithDescription(message)
                  .WithColor(WarningColor);
      return new DiscordWebhookBuilder().AddEmbed(embed);
    }
  }
}