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
			                   .WithTitle(Lang.FeedbackReceived.Translate())
			                   .WithCustomTimestamp()
			                   .WithGuildInfoFooter()
			                   .AddField(Lang.Star.Translate(), Lang.StarEmoji.Translate() + ticket.FeedbackStar)
			                   .WithColor(ModmailColors.FeedbackColor);
			if (!string.IsNullOrEmpty(ticket.FeedbackMessage)) feedbackDone.AddField(Lang.Feedback.Translate(), ticket.FeedbackMessage);

			return feedbackDone;
		}


		public static DiscordMessageBuilder YourTicketHasBeenClosed(Database.Entities.Ticket ticket, Option option, Uri? transcriptUri) {
			var messageBuilder = new DiscordMessageBuilder();
			var embedBuilder = new DiscordEmbedBuilder()
			                   .WithTitle(Lang.YourTicketHasBeenClosed.Translate())
			                   .WithDescription(Lang.YourTicketHasBeenClosedDescription.Translate())
			                   .WithGuildInfoFooter(option)
			                   .WithCustomTimestamp()
			                   .WithColor(ModmailColors.TicketClosedColor);

			var closingMessage = Lang.ClosingMessageDescription.Translate();

			if (!string.IsNullOrEmpty(closingMessage)) embedBuilder.WithDescription(closingMessage);

			if (!string.IsNullOrEmpty(ticket.CloseReason)) embedBuilder.AddField(Lang.CloseReason.Translate(), ticket.CloseReason);

			if (transcriptUri is not null) messageBuilder.AddComponents(new DiscordLinkButtonComponent(transcriptUri.AbsoluteUri, Lang.Transcript.Translate()));

			messageBuilder.AddEmbed(embedBuilder);
			return messageBuilder;
		}

		public static DiscordMessageBuilder GiveFeedbackMessage(Database.Entities.Ticket ticket, Option option) {
			var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
			var starList = new List<DiscordComponent> {
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, ticket.Id), "1", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, ticket.Id), "2", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, ticket.Id), "3", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, ticket.Id), "4", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, ticket.Id), "5", false, new DiscordComponentEmoji("⭐"))
			};

			var ticketFeedbackEmbed = new DiscordEmbedBuilder()
			                          .WithTitle(Lang.Feedback.Translate())
			                          .WithDescription(Lang.FeedbackDescription.Translate())
			                          .WithCustomTimestamp()
			                          .WithGuildInfoFooter(option)
			                          .WithColor(ModmailColors.FeedbackColor);

			var response = ticketFeedbackMsgToUser
			               .AddEmbed(ticketFeedbackEmbed)
			               .AddComponents(starList);
			return response;
		}

		public static DiscordEmbedBuilder TicketPriorityChanged(Option option, UserInformation information, Database.Entities.Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
			var embed = new DiscordEmbedBuilder()
			            .WithGuildInfoFooter(option)
			            .WithTitle(Lang.TicketPriorityChanged.Translate())
			            .WithCustomTimestamp()
			            .WithColor(ModmailColors.TicketPriorityChangedColor)
			            .AddField(Lang.OldPriority.Translate(), oldPriority.ToString(), true)
			            .AddField(Lang.NewPriority.Translate(), newPriority.ToString(), true);
			if (!ticket.Anonymous) embed.WithUserAsAuthor(information);
			// else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
			return embed;
		}


		public static DiscordMessageBuilder YouHaveCreatedNewTicket(DiscordGuild guild,
		                                                            Option option,
		                                                            List<TicketType> ticketTypes,
		                                                            Guid ticketId) {
			var embed = new DiscordEmbedBuilder()
			            .WithTitle(Lang.YouHaveCreatedNewTicket.Translate())
			            .WithFooter(guild.Name, guild.IconUrl)
			            .WithCustomTimestamp()
			            .WithColor(ModmailColors.TicketCreatedColor);
			var greetingMessage = Lang.GreetingMessageDescription.Translate();
			if (!string.IsNullOrEmpty(greetingMessage))
				embed.WithDescription(greetingMessage);

			var builder = new DiscordMessageBuilder()
				.AddEmbed(embed);

			if (ticketTypes.Count > 0) {
				var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
				                                           Lang.PleaseSelectATicketType.Translate(),
				                                           ticketTypes.Select(x => string.IsNullOrWhiteSpace(x.Emoji)
					                                                                   ? new DiscordSelectComponentOption(x.Name,
					                                                                                                      x.Key.ToString(),
					                                                                                                      x.Description)
					                                                                   : new DiscordSelectComponentOption(x.Name,
					                                                                                                      x.Key.ToString(),
					                                                                                                      x.Description,
					                                                                                                      false,
					                                                                                                      new DiscordComponentEmoji(x.Emoji)))
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

			if (!anonymous && message.Author is not null) embed.WithUserAsAuthor(message.Author);

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

			if (!anonymous && message.Author is not null) embed.WithUserAsAuthor(message.Author);

			return embed;
		}
	}

	public static class Ticket
	{
		public static DiscordMessageBuilder NewTicket(DiscordUser member, Guid ticketId) {
			var embed = new DiscordEmbedBuilder()
			            .WithTitle(Lang.NewTicket.Translate())
			            .WithCustomTimestamp()
			            .WithDescription(Lang.NewTicketDescriptionMessage.Translate())
			            .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
			            .AddField(Lang.User.Translate(), member.Mention, true)
			            .AddField(Lang.TicketId.Translate(), ticketId.ToString().ToUpper(), true)
			            .WithColor(ModmailColors.TicketCreatedColor);

			var messageBuilder = new DiscordMessageBuilder()
			                     .AddEmbed(embed)
			                     .AddComponents(new DiscordButtonComponent(DiscordButtonStyle.Danger,
			                                                               UtilInteraction.BuildKey("close_ticket_with_reason", ticketId.ToString()),
			                                                               Lang.CloseTicket.Translate(),
			                                                               emoji: new DiscordComponentEmoji("🔒"))
			                                   );
			return messageBuilder;
		}


		public static DiscordEmbedBuilder AnonymousToggled(Database.Entities.Ticket ticket) {
			var embed2 = new DiscordEmbedBuilder()
			             .WithTitle(ticket.Anonymous
				                        ? Lang.AnonymousModOn.Translate()
				                        : Lang.AnonymousModOff.Translate())
			             .WithColor(ModmailColors.AnonymousToggledColor)
			             .WithCustomTimestamp()
			             .WithDescription(ticket.Anonymous
				                              ? Lang.TicketSetAnonymousDescription.Translate()
				                              : Lang.TicketSetNotAnonymousDescription.Translate());

			if (ticket.OpenerUser is not null) embed2.WithUserAsAuthor(ticket.OpenerUser);


			return embed2;
		}

		public static DiscordEmbedBuilder TicketTypeChanged(UserInformation user, TicketType? ticketType) {
			var embed = new DiscordEmbedBuilder()
			            .WithTitle(Lang.TicketTypeChanged.Translate())
			            .WithUserAsAuthor(user)
			            .WithCustomTimestamp()
			            .WithColor(ModmailColors.TicketTypeChangedColor);
			if (ticketType is not null)
				embed.WithDescription(string.Format(Lang.TicketTypeSet.Translate(), ticketType.Emoji, ticketType.Name));
			else
				embed.WithDescription(Lang.TicketTypeRemoved.Translate());

			return embed;
		}

		public static DiscordEmbedBuilder TicketPriorityChanged(UserInformation modUser, Database.Entities.Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
			var embed = new DiscordEmbedBuilder()
			            .WithTitle(Lang.TicketPriorityChanged.Translate())
			            .WithColor(ModmailColors.TicketPriorityChangedColor)
			            .WithCustomTimestamp()
			            .AddField(Lang.OldPriority.Translate(), oldPriority.ToString(), true)
			            .AddField(Lang.NewPriority.Translate(), newPriority.ToString(), true)
			            .WithUserAsAuthor(modUser);
			return embed;
		}

		public static DiscordMessageBuilder MessageReceived(DiscordMessage message,
		                                                    TicketMessageAttachment[] attachments) {
			var embed = new DiscordEmbedBuilder()
			            .WithDescription(message.Content)
			            .WithCustomTimestamp()
			            .WithColor(ModmailColors.MessageReceivedColor);

			if (message.Author is not null) embed.WithUserAsAuthor(message.Author);

			var msgBuilder = new DiscordMessageBuilder()
			                 .AddEmbed(embed)
			                 .AddAttachments(attachments);
			return msgBuilder;
		}

		public static DiscordEmbedBuilder MessageEdited(DiscordMessage message) {
			var embed = new DiscordEmbedBuilder()
			            .WithDescription(message.Content)
			            .WithCustomTimestamp()
			            .WithColor(ModmailColors.MessageReceivedColor);

			if (message.Author is not null) embed.WithUserAsAuthor(message.Author);

			return embed;
		}
	}
}