using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Features.User.Commands;
using Serilog;

namespace Modmail.NET.Features.DiscordBot.Events;

public class OnMessageReactionAddedEvent
{
	[PerformanceLoggerAspect]
	public static async Task OnMessageReactionAdded(
		DiscordClient client,
		MessageReactionAddedEventArgs args
	) {
		if (args.User.IsBot) {
			Log.Debug(
			          "[{Source}] Ignoring reaction added by bot. UserId: {UserId}",
			          nameof(OnMessageReactionAddedEvent),
			          args.User.Id
			         );
			return;
		}

		if (args.Emoji.Name == TicketConstants.ProcessedReactionDiscordEmojiUnicode) {
			Log.Debug(
			          "[{Source}] Ignoring processed reaction emoji. EmojiName: {EmojiName}",
			          nameof(OnMessageReactionAddedEvent),
			          args.Emoji.Name
			         );
			return;
		}

		using var scope = client.ServiceProvider.CreateScope();
		var sender = scope.ServiceProvider.GetRequiredService<ISender>();
		await sender.Send(new UpdateDiscordUserCommand(args.User));

		if (args.Channel.IsPrivate)
			await ProcessPrivateChannelReaction(client, scope, args);
		else
			await ProcessTicketChannelReaction(client, scope, args);
	}

	private static async Task ProcessPrivateChannelReaction(
		DiscordClient client,
		IServiceScope scope,
		MessageReactionAddedEventArgs args
	) {
		Log.Debug(
		          "[{Source}] Processing reaction added in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
		          nameof(OnMessageReactionAddedEvent),
		          args.Channel.Id,
		          args.Message.Id,
		          args.Emoji.Name
		         );

		if (args.Message.Author?.Id != client.CurrentUser.Id) {
			Log.Debug(
			          "[{Source}] Ignoring reaction removal not from bot's own message in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
			          nameof(OnMessageReactionRemovedEvent),
			          args.Channel.Id,
			          args.Message.Id
			         );
			return;
		}


		if (args.User.Id == client.CurrentUser.Id) {
			Log.Debug(
			          "[{Source}] Ignoring bots reaction in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
			          nameof(OnMessageReactionAddedEvent),
			          args.Channel.Id,
			          args.Message.Id
			         );
			return;
		}

		var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
		var messageEntity = await dbContext.Messages
		                                   .FirstOrDefaultAsync(x => x.SentByMod && x.BotMessageId == args.Message.Id);
		if (messageEntity is null) {
			Log.Debug(
			          "[{Source}] No matching TicketMessage found for reaction in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
			          nameof(OnMessageReactionAddedEvent),
			          args.Channel.Id,
			          args.Message.Id
			         );
			return;
		}

		var ticket = await dbContext.Tickets
		                            .FilterActive()
		                            .FilterByOpenerUserId(args.User.Id)
		                            .FilterById(messageEntity.TicketId)
		                            .FilterByPrivateChannelId(args.Channel.Id)
		                            .FirstOrDefaultAsync();
		if (ticket is null) {
			Log.Warning(
			            "[{Source}] No active ticket found for reaction in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
			            nameof(OnMessageReactionAddedEvent),
			            args.Channel.Id,
			            args.Message.Id,
			            messageEntity.TicketId
			           );
			return;
		}

		await AddReactionToMirroredMessage(
		                                   client,
		                                   ticket.ModMessageChannelId,
		                                   messageEntity.MessageDiscordId,
		                                   args.Emoji
		                                  );
	}

	private static async Task ProcessTicketChannelReaction(
		DiscordClient client,
		IServiceScope scope,
		MessageReactionAddedEventArgs args
	) {
		Log.Debug(
		          "[{Source}] Processing reaction added in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
		          nameof(OnMessageReactionAddedEvent),
		          args.Channel.Id,
		          args.Message.Id,
		          args.Emoji.Name
		         );

		var topic = args.Channel.Topic;
		var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(topic);
		if (ticketId == Guid.Empty) {
			Log.Warning(
			            "[{Source}] Could not extract valid TicketId from channel topic. ChannelId: {ChannelId}, Topic: {Topic}",
			            nameof(OnMessageReactionAddedEvent),
			            args.Channel.Id,
			            topic
			           );
			return;
		}

		var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

		var messageEntity = await dbContext.Messages.FirstOrDefaultAsync(x =>
			                                                                 !x.SentByMod && x.BotMessageId == args.Message.Id && x.TicketId == ticketId);
		if (messageEntity is null) {
			Log.Debug(
			          "[{Source}] No matching TicketMessage found for reaction in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
			          nameof(OnMessageReactionAddedEvent),
			          args.Channel.Id,
			          args.Message.Id,
			          ticketId
			         );
			return;
		}

		var ticket = await dbContext.Tickets
		                            .FilterActive()
		                            .FilterById(ticketId)
		                            .FilterByModChannelId(args.Channel.Id)
		                            .FirstOrDefaultAsync();
		if (ticket is null) {
			Log.Warning(
			            "[{Source}] No active ticket found for reaction in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
			            nameof(OnMessageReactionAddedEvent),
			            args.Channel.Id,
			            args.Message.Id,
			            ticketId
			           );
			return;
		}

		await AddReactionToMirroredMessage(
		                                   client,
		                                   ticket.PrivateMessageChannelId,
		                                   messageEntity.MessageDiscordId,
		                                   args.Emoji
		                                  );
	}

	private static async Task AddReactionToMirroredMessage(
		DiscordClient client,
		ulong? channelId,
		ulong? messageId,
		DiscordEmoji emoji
	) {
		if (!channelId.HasValue || !messageId.HasValue) {
			Log.Warning(
			            "[{Source}] Cannot add reaction to mirrored message. Invalid ChannelId or MessageId. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
			            nameof(OnMessageReactionAddedEvent),
			            channelId,
			            messageId,
			            emoji.Name
			           );
			return;
		}

		try {
			var channel = await client.GetChannelAsync(channelId.Value);
			var message = await channel.GetMessageAsync(messageId.Value);
			await message.CreateReactionAsync(emoji);
			Log.Information(
			                "[{Source}] Processed reaction added {ChannelId} {MessageId} " +
			                "{MessageAuthor} {Emoji}",
			                nameof(OnMessageReactionAddedEvent),
			                channel.Id,
			                message.Id,
			                message.Author?.Id,
			                emoji.Name
			               );
		}
		catch (NotFoundException) {
			Log.Warning(
			            "[{Source}] Mirrored message not found, cannot add reaction. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
			            nameof(OnMessageReactionAddedEvent),
			            channelId,
			            messageId,
			            emoji.Name
			           );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] An error occurred while adding the reaction to the mirrored message. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
			          nameof(OnMessageReactionAddedEvent),
			          channelId,
			          messageId,
			          emoji.Name
			         );
		}
	}
}