using System.Globalization;
using DSharpPlus.Entities;
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

    public static DiscordEmbed TicketPriorityChanged(DiscordGuild ctxGuild, DiscordMember ticketOpenUser) {
      throw new NotImplementedException();
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

    public static DiscordEmbed MessageSent(DiscordUser author, DiscordUser user, DiscordMessage message, DiscordChannel channel) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Sent")
                  .WithFooter($"To {author.GetUsername()} | {user.Id}", author.AvatarUrl)
                  // .WithAuthor($"{author.GetUsername()} | {author.Id}", iconUrl: author.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.Green);
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);
      return embed;
    }
  }


  public static class ToLog
  {
    public static DiscordEmbed TicketCreated(DiscordUser user,
                                             DiscordMessage initialMessage,
                                             DiscordChannel mailChannel,
                                             Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("New Ticket Created")
                  .WithAuthor(user.Username, iconUrl: user.AvatarUrl)
                  .WithTimestamp(initialMessage.Timestamp)
                  .WithColor(DiscordColor.Green)
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
                                            Guid ticketId,
                                            DateTime createdAt,
                                            string reason = ""
    ) {
      if (string.IsNullOrEmpty(reason)) reason = "No reason provided";

      var embed = new DiscordEmbedBuilder()
                  .WithDescription("Ticket has been closed.")
                  .WithTimestamp(DateTime.Now)
                  .WithTitle("Ticket Closed")
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

    public static DiscordEmbed MessageSentByMod(DiscordUser author,
                                                DiscordMessage message,
                                                DiscordChannel channel,
                                                Guid ticketId,
                                                ulong guildId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Sent by Mod")
                  .WithFooter($"{author.GetUsername()} | {author.Id}", author.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.CornflowerBlue)
                  .AddField("Ticket Id", ticketId.ToString().ToUpper());
      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);

      return embed;
    }

    public static DiscordEmbed MessageSentByUser(DiscordUser author,
                                                 DiscordMessage message,
                                                 DiscordChannel channel,
                                                 Guid ticketId,
                                                 ulong guildId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle("Message Sent by User")
                  .WithFooter($"{author.GetUsername()} | {author.Id}", author.AvatarUrl)
                  .WithDescription(message.Content)
                  .WithTimestamp(message.Timestamp)
                  .WithColor(DiscordColor.CornflowerBlue)
                  .AddField("Ticket Id", ticketId.ToString().ToUpper());

      for (var i = 0; i < message.Attachments.Count; i++) embed.AddField($"Attachment {i + 1}", message.Attachments[i].Url);

      return embed;
    }
  }
}