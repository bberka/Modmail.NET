using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Entities;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Helpers;

public static class TicketBotMessages
{
	public static class User
	{
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
	}

	public static class Ticket
	{
		public static DiscordMessageBuilder NewTicket(DiscordUser member, Guid ticketId) {
			var embed = new DiscordEmbedBuilder()
			            .WithTitle(Lang.NewTicket.Translate())
			            .WithCustomTimestamp()
			            .WithDescription(Lang.NewTicketDescriptionMessage.Translate())
			            .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
			            .AddField(Lang.User.Translate(), member.Mention)
			            .AddField(Lang.TicketId.Translate(), ticketId.ToString().ToUpper())
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
	}
}