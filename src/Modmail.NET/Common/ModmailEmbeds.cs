using System.Globalization;
using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static class ModmailEmbeds
{

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
                .WithColor(DiscordColor.Gold);

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
                .WithColor(DiscordColor.Gold)
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
                .WithColor(DiscordColor.Red);
    return embed;
  }

  public static class ToUser
  {
    public static DiscordEmbed UserBlocked(DiscordUser author, DiscordGuild guild) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_BEEN_BLACKLISTED)
                  .WithDescription(Texts.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.DarkRed);
      return embed;
    }

    public static DiscordEmbed MessageSent(DiscordGuild guild,
                                           DiscordUser user,
                                           DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  // .WithTitle("Message Sent")
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .WithColor(DiscordColor.Green);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);
      return embed;
    }

    public static DiscordEmbed TicketCreated(DiscordGuild guild, DiscordUser author, DiscordMessage message, GuildOption option) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_CREATED_NEW_TICKET)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Blue);
      if (!string.IsNullOrEmpty(option.GreetingMessage))
        embed.WithDescription(option.GreetingMessage);

      return embed;
    }


    public static DiscordEmbed MessageReceived(DiscordUser author, DiscordMessage message, DiscordGuild guild, bool anonymous = false) {
      var embed = new DiscordEmbedBuilder()
                  // .WithTitle("Message Received")
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Blue);
      if (!anonymous) embed.WithAuthor(author.Username, iconUrl: author.AvatarUrl);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      return embed;
    }

    public static DiscordEmbed TicketClosed(DiscordGuild guild, DiscordUser user, GuildOption option) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOUR_TICKET_HAS_BEEN_CLOSED)
                  .WithDescription(Texts.YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.Red);
      if (!string.IsNullOrEmpty(option.ClosingMessage))
        embed.WithDescription(option.ClosingMessage);
      return embed;
    }


    public static DiscordEmbed TicketPriorityChanged(DiscordGuild guild,
                                                     DiscordUser modUser,
                                                     TicketPriority oldPriority,
                                                     TicketPriority newPriority,
                                                     bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.Magenta)
                  .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                  .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
      if (!anonymous) embed.WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl);
      return embed.Build();
    }

    public static DiscordEmbed Blacklisted(DiscordGuild guild, DiscordUser user, string? reason) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.YOU_HAVE_BEEN_BLACKLISTED )
                  .WithDescription(Texts.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION )
                  .WithFooter(guild.Name, guild.IconUrl)
                  // .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
                  .WithColor(DiscordColor.Red);
      if (!string.IsNullOrEmpty(reason)) embed.AddField(Texts.CLOSE_REASON, reason);
      return embed;
    }
  }

  public static class ToMail
  {
    public static DiscordEmbed NewTicket(DiscordMember member) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NEW_TICKET)
                  .WithTimestamp(DateTime.Now)
                  .WithDescription(Texts.NEW_TICKET_DESCRIPTION_MESSAGE)
                  .WithFooter($"{member.GetUsername()} | {member.Id}", member.AvatarUrl)
                  .AddField(Texts.USER, member.Mention, true)
                  .WithColor(DiscordColor.Green);
      if (member.Roles is not null) {
        var str = string.Join(", ", member.Roles.Select(x => x.Mention));
        if (!string.IsNullOrEmpty(str)) embed.AddField(Texts.ROLES, str, true);
      }

      return embed;
    }

    public static DiscordEmbed MessageReceived(DiscordUser user,
                                               DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  // .WithTitle("Message Received")
                  .WithAuthor(user.GetUsername(), null, user.AvatarUrl)
                  // .WithFooter(, user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Blue);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);
      return embed;
    }

    public static DiscordEmbed MessageSent(DiscordUser author, DiscordUser user, DiscordMessage message, DiscordChannel channel, bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  // .WithTitle("Message Sent")
                  // .WithFooter($"To {user.GetUsername()} | {user.Id}", user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Green);
      embed.WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl);

      for (var i = 0; i < message.Attachments.Count; i++)
        embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      if (ticketAnonymous) embed.WithFooter(Texts.ANONYMOUS_MESSAGE);

      return embed;
    }

    public static DiscordEmbed NoteAdded(DiscordGuild guild, DiscordUser ctxUser, string note) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NOTE_ADDED)
                  .WithDescription(note)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(ctxUser.GetUsername(), iconUrl: ctxUser.AvatarUrl);
      return embed;
    }

    public static DiscordEmbed AnonymousToggled(DiscordGuild guild, DiscordUser user, Ticket ticket, bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.ANONYMOUS_TOGGLED)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                  .AddField(Texts.TOGGLED_BY, user.Mention + " | " + user.GetUsername() + " | " + user.Id);
      embed.WithDescription(ticketAnonymous
                              ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                              : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);
      return embed;
    }
  }


  public static class ToLog
  {
    public static DiscordEmbed TicketCreated(DiscordUser user,
                                             DiscordMessage initialMessage,
                                             DiscordChannel mailChannel,
                                             DiscordGuild guild,
                                             Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NEW_TICKET_CREATED)
                  .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
                  .WithTimestamp(initialMessage.Timestamp)
                  .WithColor(DiscordColor.Green)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .AddField(Texts.USER, user.Mention, true)
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.USERNAME, user.GetUsername(), true)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), true)
                  .AddField(Texts.TICKET_CREATED_AT, initialMessage.CreationTimestamp.ToString(), true)
                  .AddField(Texts.CHANNEL_ID, mailChannel.Id.ToString(), true);
      return embed;
    }

    public static DiscordEmbed TicketClosed(DiscordUser mailCloserUser,
                                            DiscordUser mailCreatorUser,
                                            DiscordGuild guild,
                                            Guid ticketId,
                                            DateTime createdAt,
                                            string? reason = null
    ) {
      if (string.IsNullOrEmpty(reason)) reason = Texts.NO_REASON_PROVIDED;

      var embed = new DiscordEmbedBuilder()
                  // .WithDescription("Ticket has been closed.")
                  .WithTimestamp(DateTime.Now)
                  .WithTitle(Texts.TICKET_CLOSED)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithAuthor(mailCloserUser.GetUsername(), iconUrl: mailCloserUser.AvatarUrl)
                  .WithColor(DiscordColor.Red)
                  .AddField(Texts.OPENED_BY_USER, mailCreatorUser.Mention, true)
                  .AddField(Texts.OPENED_BY_USER_ID, mailCreatorUser.Id.ToString(), true)
                  .AddField(Texts.OPENED_BY_USERNAME, mailCreatorUser.GetUsername(), true)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), true)
                  .AddField(Texts.OPENED_AT, createdAt.ToString(CultureInfo.InvariantCulture), true)
                  .AddField(Texts.CLOSED_BY, mailCloserUser.Mention, true)
                  .AddField(Texts.CLOSE_REASON, reason, true);
      return embed;
    }


    public static DiscordEmbed TicketPriorityChanged(DiscordGuild guild,
                                                     DiscordUser modUser,
                                                     TicketPriority oldPriority,
                                                     TicketPriority newPriority,
                                                     bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.Magenta)
                  .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                  .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
      if (!anonymous) embed.WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl);
      return embed.Build();
    }

    public static DiscordEmbed MessageSentByMod(DiscordUser mod,
                                                DiscordUser user,
                                                DiscordMessage message,
                                                DiscordChannel channel,
                                                Guid ticketId,
                                                ulong guildId,
                                                bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.MESSAGE_SENT_BY_MOD)
                  .WithAuthor(mod.GetUsername(), null, mod.AvatarUrl)
                  // .WithFooter("To " + user.GetUsername() + " | " + user.Id, user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Cyan)
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
                                                 DiscordChannel channel,
                                                 Guid ticketId,
                                                 ulong guildId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.MESSAGE_SENT_BY_USER)
                  .WithAuthor(user.GetUsername(), null, user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.CornflowerBlue)
                  .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper())
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.CHANNEL_ID, channel.Id.ToString(), true)
        ;

      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);

      return embed;
    }

    public static DiscordEmbed NoteAdded(DiscordGuild guild, DiscordUser user, string note, Ticket ticket) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NOTE_ADDED)
                  .WithDescription(note)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
        ;
      return embed;
    }

    public static DiscordEmbed AnonymousToggled(DiscordGuild guild, DiscordUser user, Ticket ticket, bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.ANONYMOUS_TOGGLED)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                  .AddField(Texts.TOGGLED_BY, user.Mention + " | " + user.GetUsername() + " | " + user.Id);

      embed.WithDescription(anonymous
                              ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                              : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);

      return embed;
    }

    public static DiscordEmbed BlacklistAdded(DiscordGuild guild, DiscordUser author, DiscordUser user, string? reason) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.USER_BLACKLISTED)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
                  .WithColor(DiscordColor.Yellow)
                  .AddField(Texts.USER, user.Mention, true)
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.USERNAME, user.GetUsername(), true);

      return embed;
    }

    public static DiscordEmbed BlacklistRemoved(DiscordGuild guild, DiscordUser author, DiscordUser user) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.USER_BLACKLIST_REMOVED)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
                  .WithColor(DiscordColor.Orange)
                  .AddField(Texts.USER, user.Mention, true)
                  .AddField(Texts.USER_ID, user.Id.ToString(), true)
                  .AddField(Texts.USERNAME, user.GetUsername(), true);
      return embed;
    }

    public static DiscordEmbed FeedbackReceived(int starCount, string textInput, DiscordGuild mainGuild, DiscordUser interactionUser) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.FEEDBACK_RECEIVED)
                  .WithDescription(textInput)
                  .WithTimestamp(DateTime.Now)
                  .WithFooter(mainGuild.Name, mainGuild.IconUrl)
                  .AddField(Texts.STAR, starCount.ToString(), true)
                  .AddField(Texts.USER, interactionUser.Mention, true)
                  .AddField(Texts.USER_ID, interactionUser.Id.ToString(), true)
                  .WithColor(DiscordColor.Orange)
                  .WithAuthor(interactionUser.GetUsername(), iconUrl: interactionUser.AvatarUrl);
      return embed;
    }
  }

  public static class Interaction
  {
    public static DiscordEmbed EmbedFeedback(DiscordGuild guild) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.FEEDBACK)
                  .WithDescription(Texts.FEEDBACK_DESCRIPTION)
                  .WithTimestamp(DateTime.Now)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithColor(DiscordColor.Teal);
      return embed;
    }

    public static DiscordEmbed EmbedFeedbackDone(int starCount, string textInput, DiscordGuild guild) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.FEEDBACK_RECEIVED)
                  .WithTimestamp(DateTime.Now)
                  .WithFooter(guild.Name, guild.IconUrl)
                  .AddField(Texts.STAR, Texts.STAR_EMOJI + starCount)
                  .AddField(Texts.FEEDBACK, textInput)
                  .WithColor(DiscordColor.Teal);
      return embed;
    }
  }
}