﻿using System.Globalization;
using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static class ModmailEmbedBuilder
{
  public static class ToUser
  {
    public static DiscordEmbed MessageSent(DiscordGuild guild,
                                           DiscordUser user,
                                           DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Sent")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Green);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);
      return embed;
    }

    public static DiscordEmbed TicketCreated(DiscordGuild guild, DiscordUser author, DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Ticket Created")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Green);
      return embed;
    }


    public static DiscordEmbed MessageReceived(DiscordUser author, DiscordMessage message, DiscordGuild guild, bool anonymous = false) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Received")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Blue);
      if (!anonymous) embed.WithAuthor(author.Username, iconUrl: author.AvatarUrl);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);

      return embed;
    }

    public static DiscordEmbed TicketClosed(DiscordGuild guild, DiscordUser user) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Ticket Closed")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.Red)
                  .AddField("Note", "You can always open another ticket by just messaging me again.");
      return embed;
    }


    public static DiscordEmbed TicketPriorityChanged(DiscordGuild guild,
                                                     DiscordUser modUser,
                                                     TicketPriority oldPriority,
                                                     TicketPriority newPriority) {
      var embed = new DiscordEmbedBuilder()
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl)
                  .WithDescription("Ticket priority has been changed.")
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.Magenta)
                  .AddField("Old Priority", oldPriority.ToString(), true)
                  .AddField("New Priority", newPriority.ToString(), true);
      return embed.Build();
    }
  }

  public static class ToMail
  {
    public static DiscordEmbed NewTicket(DiscordMember member) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("New Ticket")
                  .WithTimestamp(DateTime.Now)
                  .WithDescription("New ticket has been created. Please respond to this message to continue the conversation."
                                   + Environment.NewLine
                                   + Environment.NewLine
                                   + "If you want to close the ticket, you can use the `/ticket close` command."
                                   + Environment.NewLine
                                   + Environment.NewLine
                                   + "If you want to change the priority of the ticket, you can use the `/ticket set-priority` command."
                                   + Environment.NewLine
                                   + Environment.NewLine
                                   + "If you want to add a note to the ticket, you can use the `/ticket add-note` command."
                                   + Environment.NewLine
                                   + Environment.NewLine
                                   + $"Messages starting with bot prefix `{MMConfig.This.BotPrefix}` are ignored, can be used for staff discussion. ")
                  .WithFooter($"{member.GetUsername()} | {member.Id}", member.AvatarUrl)
                  .AddField("User", member.Mention + " | " + member.Id, true)
                  .WithColor(DiscordColor.Green);
      if (member.Roles is not null) {
        var str = string.Join(", ", member.Roles.Select(x => x.Mention));
        if (!string.IsNullOrEmpty(str)) {
          embed.AddField("Roles", str, true);
        }
      }

      return embed;
    }

    public static DiscordEmbed MessageReceived(DiscordUser user,
                                               DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Received")
                  .WithFooter($"{user.GetUsername()} | {user.Id}", user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Blue);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);
      return embed;
    }

    public static DiscordEmbed MessageSent(DiscordUser author, DiscordUser user, DiscordMessage message, DiscordChannel channel, bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Sent")
                  .WithFooter($"To {user.GetUsername()} | {user.Id}", user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Green);
      for (var i = 0; i < message.Attachments.Count; i++)
        embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);

      if (!ticketAnonymous) {
        embed.WithAuthor($"{author.GetUsername()} | {author.Id}", iconUrl: author.AvatarUrl);
      }

      return embed;
    }

    public static DiscordEmbed NoteAdded(DiscordGuild ctxGuild, DiscordUser ctxUser, string note) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Note Added")
                  .WithDescription(note)
                  .WithFooter($"{ctxGuild.Name} | {ctxGuild.Id}", ctxGuild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(ctxUser.GetUsername(), iconUrl: ctxUser.AvatarUrl);
      return embed;
    }

    public static DiscordEmbed AnonymousToggled(DiscordGuild guild, DiscordUser user, Ticket ticket, bool ticketAnonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Anonymous Toggled")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField("Ticket Id", ticket.Id.ToString().ToUpper(), false)
                  .AddField("Toggled By", user.Mention + " | " + user.GetUsername() + " | " + user.Id, false);
      embed.WithDescription(ticketAnonymous
                              ? "This ticket is now anonymous. The member will not know who is responding to their messages."
                              : "This ticket is no longer anonymous. The member can see who is responding to their messages.");
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
                  .WithTitle("New Ticket Created")
                  .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
                  .WithTimestamp(initialMessage.Timestamp)
                  .WithColor(DiscordColor.Green)
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .AddField("User", user.Mention, true)
                  .AddField("User Id", user.Id.ToString(), true)
                  .AddField("Username", user.GetUsername(), true)
                  .AddField("Ticket Id", ticketId.ToString().ToUpper(), true)
                  .AddField("Ticket Created At", initialMessage.CreationTimestamp.ToString(), true)
                  .AddField("Channel Id", mailChannel.Id.ToString(), true);
      return embed;
    }

    public static DiscordEmbed TicketClosed(DiscordUser mailCloserUser,
                                            DiscordUser mailCreatorUser,
                                            DiscordGuild guild,
                                            Guid ticketId,
                                            DateTime createdAt,
                                            string reason = ""
    ) {
      if (string.IsNullOrEmpty(reason)) reason = "No reason provided";

      var embed = new DiscordEmbedBuilder()
                  .WithDescription("Ticket has been closed.")
                  .WithTimestamp(DateTime.Now)
                  .WithTitle("Ticket Closed")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithColor(DiscordColor.Red)
                  .AddField("Opened By User", mailCreatorUser.Mention, true)
                  .AddField("Opened By User Id", mailCreatorUser.Id.ToString(), true)
                  .AddField("Opened By Username", mailCreatorUser.GetUsername(), true)
                  .AddField("Ticket Id", ticketId.ToString().ToUpper(), true)
                  .AddField("Opened At", createdAt.ToString(CultureInfo.InvariantCulture), true)
                  .AddField("Closed By", mailCloserUser.Mention, true)
                  .AddField("Reason", reason, true);
      return embed;
    }


    public static DiscordEmbed TicketPriorityChanged(DiscordGuild guild,
                                                     DiscordUser modUser,
                                                     TicketPriority oldPriority,
                                                     TicketPriority newPriority) {
      var embed = new DiscordEmbedBuilder()
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl)
                  .WithDescription("Ticket priority has been changed.")
                  .WithTimestamp(DateTime.Now)
                  .WithColor(DiscordColor.Magenta)
                  .AddField("Old Priority", oldPriority.ToString(), true)
                  .AddField("New Priority", newPriority.ToString(), true);
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
                  .WithTitle("Message Sent by Mod")
                  .WithAuthor($"{mod.GetUsername()} | {mod.Id}", mod.AvatarUrl)
                  .WithFooter("To " + user.GetUsername() + " | " + user.Id, user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.CornflowerBlue)
                  .AddField("Ticket Id", ticketId.ToString().ToUpper());
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);

      if (ticketAnonymous) {
        embed.AddField("Anonymous", "This message was sent anonymously.");
      }

      return embed;
    }

    public static DiscordEmbed MessageSentByUser(DiscordUser user,
                                                 DiscordMessage message,
                                                 DiscordChannel channel,
                                                 Guid ticketId,
                                                 ulong guildId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Sent by User")
                  .WithAuthor($"{user.GetUsername()} | {user.Id}", user.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.CornflowerBlue)
                  .AddField("Ticket Id", ticketId.ToString().ToUpper());

      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);

      return embed;
    }

    public static DiscordEmbed NoteAdded(DiscordGuild ctxGuild, DiscordUser ctxUser, string note, Ticket ticket) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Note Added")
                  .WithDescription(note)
                  .WithFooter($"{ctxGuild.Name} | {ctxGuild.Id}", ctxGuild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(ctxUser.GetUsername(), iconUrl: ctxUser.AvatarUrl)
                  .AddField("Ticket Id", ticket.Id.ToString().ToUpper(), false)
                  .AddField("Note Added By", ctxUser.Mention + " | " + ctxUser.GetUsername() + " | " + ctxUser.Id, false)
        ;
      return embed;
    }

    public static DiscordEmbed AnonymousToggled(DiscordGuild guild, DiscordUser user, Ticket ticket, bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Anonymous Toggled")
                  .WithFooter($"{guild.Name} | {guild.Id}", guild.IconUrl)
                  .WithColor(DiscordColor.Gold)
                  .WithTimestamp(DateTime.Now)
                  .WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl)
                  .AddField("Ticket Id", ticket.Id.ToString().ToUpper(), false)
                  .AddField("Toggled By", user.Mention + " | " + user.GetUsername() + " | " + user.Id, false);

      embed.WithDescription(anonymous
                              ? "This ticket is now anonymous. The member will not know who is responding to their messages."
                              : "This ticket is no longer anonymous. The member can see who is responding to their messages.");

      return embed;
    }
  }

  public static DiscordEmbed ListTags(DiscordGuild ctxGuild, List<Tag> tags) {
    var sb = new StringBuilder();
    foreach (var tag in tags) sb.AppendLine(tag.Key);
    var embed = new DiscordEmbedBuilder()
                .WithTitle("Tag List")
                .WithDescription(sb.ToString())
                .WithFooter($"{ctxGuild.Name} | {ctxGuild.Id}", ctxGuild.IconUrl)
                .WithColor(DiscordColor.Gold);
    return embed;
  }

  public static DiscordEmbed Base(string title, string text = "", DiscordColor? color = null) {
    color ??= DiscordColor.White;
    var embed = new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithColor(color.Value);
    if (!string.IsNullOrEmpty(text))
      embed.WithDescription(text);
    return embed;
  }

  public static DiscordEmbed ListTeams(DiscordGuild ctxGuild, List<GuildTeam> teams) {
    // var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithTitle("Team List")
                // .WithDescription(sb.ToString())
                .WithFooter($"{ctxGuild.Name} | {ctxGuild.Id}", ctxGuild.IconUrl)
                .WithColor(DiscordColor.Gold);

    foreach (var team in teams) {
      var sb = new StringBuilder();
      sb.AppendLine($"`Enabled`: {team.IsEnabled}");
      sb.AppendLine($"`Members`: {team.GuildTeamMembers.Count}");
      foreach (var member in team.GuildTeamMembers.OrderBy(x => x.Type)) {
        switch (member.Type) {
          case TeamMemberDataType.RoleId:
            //Tag role 
            sb.AppendLine($"`Role`: <@&{member.Key}>");
            break;
          case TeamMemberDataType.UserId:
            //tag member
            sb.AppendLine($"`User`: <@{member.Key}>");
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      embed.AddField(team.Name, sb.ToString());
    }

    return embed;
  }

  public static DiscordEmbed Settings(DiscordGuild guild, GuildOption ticketOption) {
    var sb = new StringBuilder();
    var embed = new DiscordEmbedBuilder()
                .WithTitle("Modmail Settings")
                .WithColor(DiscordColor.Gold)
                .WithFooter(guild.Name + " | " + guild.Id, guild.IconUrl);

    sb.AppendLine("`Modmail Enabled`: " + ticketOption.IsEnabled);
    sb.AppendLine("`Sensitive Logging`: " + ticketOption.IsSensitiveLogging);
    sb.AppendLine("`Take Feedback After Closing`: " + ticketOption.TakeFeedbackAfterClosing);
    sb.AppendLine("`Show Confirmations`: " + ticketOption.ShowConfirmationWhenClosingTickets);
    // sb.AppendLine("`Allow Anonymous Response`: " + ticketOption.AllowAnonymousResponding);
    sb.AppendLine("`Log Channel`: <#" + ticketOption.LogChannelId + "> | " + ticketOption.LogChannelId);
    sb.AppendLine("`Tickets Category Id`: <#" + ticketOption.CategoryId + "> | " + ticketOption.CategoryId);
    embed.WithDescription(sb.ToString());
    return embed;
  }
}