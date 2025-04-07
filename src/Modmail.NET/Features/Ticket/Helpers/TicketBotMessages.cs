using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Helpers;

public static class TicketBotMessages
{
  public static class User
  {
    public static DiscordEmbedBuilder FeedbackReceivedUpdateMessage(Database.Entities.Ticket ticket) {
      var feedbackDone = new DiscordEmbedBuilder()
                         .WithTitle(LangKeys.FeedbackReceived.GetTranslation())
                         .WithCustomTimestamp()
                         .WithGuildInfoFooter()
                         .AddField(LangKeys.Star.GetTranslation(), LangKeys.StarEmoji.GetTranslation() + ticket.FeedbackStar)
                         .AddField(LangKeys.Feedback.GetTranslation(), ticket.FeedbackMessage)
                         .WithColor(ModmailColors.FeedbackColor);
      return feedbackDone;
    }


    public static DiscordMessageBuilder YourTicketHasBeenClosed(Database.Entities.Ticket ticket, GuildOption guildOption, Uri transcriptUri) {
      var messageBuilder = new DiscordMessageBuilder();
      var embedBuilder = new DiscordEmbedBuilder()
                         .WithTitle(LangKeys.YourTicketHasBeenClosed.GetTranslation())
                         .WithDescription(LangKeys.YourTicketHasBeenClosedDescription.GetTranslation())
                         .WithGuildInfoFooter(guildOption)
                         .WithCustomTimestamp()
                         .WithColor(ModmailColors.TicketClosedColor);

      var closingMessage = LangKeys.ClosingMessageDescription.GetTranslation();

      if (!string.IsNullOrEmpty(closingMessage)) embedBuilder.WithDescription(closingMessage);

      if (!string.IsNullOrEmpty(ticket.CloseReason)) embedBuilder.AddField(LangKeys.CloseReason.GetTranslation(), ticket.CloseReason);

      if (transcriptUri is not null) messageBuilder.AddComponents(new DiscordLinkButtonComponent(transcriptUri.AbsoluteUri, LangKeys.Transcript.GetTranslation()));

      messageBuilder.AddEmbed(embedBuilder);
      return messageBuilder;
    }

    public static DiscordMessageBuilder GiveFeedbackMessage(Database.Entities.Ticket ticket, GuildOption guildOption) {
      var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
      var starList = new List<DiscordComponent> {
        new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, ticket.Id), "1", false, new DiscordComponentEmoji("⭐")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, ticket.Id), "2", false, new DiscordComponentEmoji("⭐")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, ticket.Id), "3", false, new DiscordComponentEmoji("⭐")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, ticket.Id), "4", false, new DiscordComponentEmoji("⭐")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, ticket.Id), "5", false, new DiscordComponentEmoji("⭐"))
      };

      var ticketFeedbackEmbed = new DiscordEmbedBuilder()
                                .WithTitle(LangKeys.Feedback.GetTranslation())
                                .WithDescription(LangKeys.FeedbackDescription.GetTranslation())
                                .WithCustomTimestamp()
                                .WithGuildInfoFooter(guildOption)
                                .WithColor(ModmailColors.FeedbackColor);

      var response = ticketFeedbackMsgToUser
                     .AddEmbed(ticketFeedbackEmbed)
                     .AddComponents(starList);
      return response;
    }

    public static DiscordEmbedBuilder TicketPriorityChanged(GuildOption guildOption, DiscordUserInfo info, Database.Entities.Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
      var embed = new DiscordEmbedBuilder()
                  .WithGuildInfoFooter(guildOption)
                  .WithTitle(LangKeys.TicketPriorityChanged.GetTranslation())
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.TicketPriorityChangedColor)
                  .AddField(LangKeys.OldPriority.GetTranslation(), oldPriority.ToString(), true)
                  .AddField(LangKeys.NewPriority.GetTranslation(), newPriority.ToString(), true);
      if (!ticket.Anonymous) embed.WithUserAsAuthor(info);
      // else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
      return embed;
    }


    public static DiscordMessageBuilder YouHaveCreatedNewTicket(DiscordGuild guild,
                                                                GuildOption option,
                                                                List<TicketType> ticketTypes,
                                                                Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(LangKeys.YouHaveCreatedNewTicket.GetTranslation())
                  .WithFooter(guild.Name, guild.IconUrl)
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.TicketCreatedColor);
      var greetingMessage = LangKeys.GreetingMessageDescription.GetTranslation();
      if (!string.IsNullOrEmpty(greetingMessage))
        embed.WithDescription(greetingMessage);

      var builder = new DiscordMessageBuilder()
        .AddEmbed(embed);

      if (ticketTypes.Count > 0) {
        var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                                                   LangKeys.PleaseSelectATicketType.GetTranslation(),
                                                   ticketTypes.Select(x => new DiscordSelectComponentOption(x.Name,
                                                                                                            x.Key.ToString(),
                                                                                                            x.Description,
                                                                                                            false,
                                                                                                            !string.IsNullOrWhiteSpace(x.Emoji)
                                                                                                              ? new DiscordComponentEmoji(x.Emoji)
                                                                                                              : null))
                                                              .ToList());
        builder.AddComponents(selectBox);
      }

      return builder;
    }


    public static DiscordMessageBuilder MessageReceived(DiscordMessage message, TicketMessageAttachment[] attachments, bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithDescription(message.Content)
                  .WithGuildInfoFooter()
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.MessageReceivedColor);

      if (!anonymous) embed.WithUserAsAuthor(message.Author);

      var msg = new DiscordMessageBuilder();

      msg.AddEmbed(embed);
      msg.AddAttachments(attachments);

      return msg;
    }

    public static DiscordEmbedBuilder MessageEdited(DiscordMessage message, bool anonymous) {
      var embed = new DiscordEmbedBuilder()
                  .WithDescription(message.Content)
                  .WithGuildInfoFooter()
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.MessageReceivedColor);

      if (!anonymous) embed.WithUserAsAuthor(message.Author);

      return embed;
    }
  }

  public static class Ticket
  {
    public static DiscordMessageBuilder NewTicket(DiscordUser member, Guid ticketId) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(LangKeys.NewTicket.GetTranslation())
                  .WithCustomTimestamp()
                  .WithDescription(LangKeys.NewTicketDescriptionMessage.GetTranslation())
                  .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
                  .AddField(LangKeys.User.GetTranslation(), member.Mention, true)
                  .AddField(LangKeys.TicketId.GetTranslation(), ticketId.ToString().ToUpper(), true)
                  .WithColor(ModmailColors.TicketCreatedColor);

      var messageBuilder = new DiscordMessageBuilder()
                           .AddEmbed(embed)
                           .AddComponents(new DiscordButtonComponent(DiscordButtonStyle.Danger,
                                                                     UtilInteraction.BuildKey("close_ticket_with_reason", ticketId.ToString()),
                                                                     LangKeys.CloseTicket.GetTranslation(),
                                                                     emoji: new DiscordComponentEmoji("🔒"))
                                         );
      return messageBuilder;
    }

    public static DiscordEmbedBuilder NoteAdded(TicketNote note, DiscordUserInfo user) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(LangKeys.NoteAdded.GetTranslation())
                  .WithDescription(note.Content)
                  .WithColor(ModmailColors.NoteAddedColor)
                  .WithCustomTimestamp()
                  .WithUserAsAuthor(user);
      return embed;
    }

    public static DiscordEmbedBuilder AnonymousToggled(Database.Entities.Ticket ticket) {
      var embed2 = new DiscordEmbedBuilder()
                   .WithTitle(ticket.Anonymous
                                ? LangKeys.AnonymousModOn.GetTranslation()
                                : LangKeys.AnonymousModOff.GetTranslation())
                   .WithColor(ModmailColors.AnonymousToggledColor)
                   .WithCustomTimestamp()
                   .WithDescription(ticket.Anonymous
                                      ? LangKeys.TicketSetAnonymousDescription.GetTranslation()
                                      : LangKeys.TicketSetNotAnonymousDescription.GetTranslation());

      if (ticket.OpenerUser is not null) embed2.WithUserAsAuthor(ticket.OpenerUser);


      return embed2;
    }

    public static DiscordEmbedBuilder TicketTypeChanged(DiscordUserInfo user, TicketType ticketType) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(LangKeys.TicketTypeChanged.GetTranslation())
                  .WithUserAsAuthor(user)
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.TicketTypeChangedColor);
      if (ticketType is not null)
        embed.WithDescription(string.Format(LangKeys.TicketTypeSet.GetTranslation(), ticketType.Emoji, ticketType.Name));
      else
        embed.WithDescription(LangKeys.TicketTypeRemoved.GetTranslation());

      return embed;
    }

    public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo modUser, Database.Entities.Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(LangKeys.TicketPriorityChanged.GetTranslation())
                  .WithColor(ModmailColors.TicketPriorityChangedColor)
                  .WithCustomTimestamp()
                  .AddField(LangKeys.OldPriority.GetTranslation(), oldPriority.ToString(), true)
                  .AddField(LangKeys.NewPriority.GetTranslation(), newPriority.ToString(), true)
                  .WithUserAsAuthor(modUser);
      return embed;
    }

    public static DiscordMessageBuilder MessageReceived(DiscordMessage message,
                                                        TicketMessageAttachment[] attachments) {
      var embed = new DiscordEmbedBuilder()
                  .WithDescription(message.Content)
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.MessageReceivedColor)
                  .WithUserAsAuthor(message.Author);

      var msgBuilder = new DiscordMessageBuilder()
                       .AddEmbed(embed)
                       .AddAttachments(attachments);
      return msgBuilder;
    }

    public static DiscordEmbedBuilder MessageEdited(DiscordMessage message) {
      var embed = new DiscordEmbedBuilder()
                  .WithDescription(message.Content)
                  .WithCustomTimestamp()
                  .WithColor(ModmailColors.MessageReceivedColor)
                  .WithUserAsAuthor(message.Author);
      return embed;
    }
  }
}