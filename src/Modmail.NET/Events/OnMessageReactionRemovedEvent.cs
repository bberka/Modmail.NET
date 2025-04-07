using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Aspects;
using Modmail.NET.Database;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Events;

public static class OnMessageReactionRemovedEvent
{
  [PerformanceLoggerAspect]
  public static async Task OnMessageReactionRemoved(
    DiscordClient client,
    MessageReactionRemovedEventArgs args
  ) {
    if (args is null) {
      Log.Debug(
                "[{Source}] MessageReactionRemovedEventArgs is null, exiting",
                nameof(OnMessageReactionRemovedEvent)
               );
      return;
    }

    if (args.User.IsBot) {
      Log.Debug(
                "[{Source}] Ignoring reaction removed by bot. UserId: {UserId}",
                nameof(OnMessageReactionRemovedEvent),
                args.User.Id
               );
      return;
    }

    if (args.Emoji.Name == Const.ProcessedReactionDiscordEmojiUnicode) {
      Log.Debug(
                "[{Source}] Ignoring processed reaction emoji removed. EmojiName: {EmojiName}",
                nameof(OnMessageReactionRemovedEvent),
                args.Emoji.Name
               );
      return;
    }

    using var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.User));

    if (args.Channel.IsPrivate)
      await ProcessPrivateChannelReactionRemoval(client, scope, args);
    else
      await ProcessTicketChannelReactionRemoval(client, scope, args);
  }

  private static async Task ProcessPrivateChannelReactionRemoval(
    DiscordClient client,
    IServiceScope scope,
    MessageReactionRemovedEventArgs args
  ) {
    Log.Debug(
              "[{Source}] Processing reaction removed in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
              nameof(OnMessageReactionRemovedEvent),
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
    var messageEntity = await dbContext.TicketMessages
                                       .FirstOrDefaultAsync(x => x.SentByMod && x.BotMessageId == args.Message.Id);
    if (messageEntity is null) {
      Log.Debug(
                "[{Source}] No matching TicketMessage found for reaction removal in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
                nameof(OnMessageReactionRemovedEvent),
                args.Channel.Id,
                args.Message.Id
               );
      return;
    }

    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x =>
                                                               !x.ClosedDateUtc.HasValue && x.OpenerUserId == args.User.Id &&
                                                               x.Id == messageEntity.TicketId && x.PrivateMessageChannelId == args.Channel.Id);
    if (ticket is null) {
      Log.Warning(
                  "[{Source}] No active ticket found for reaction removal in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
                  nameof(OnMessageReactionRemovedEvent),
                  args.Channel.Id,
                  args.Message.Id,
                  messageEntity.TicketId
                 );
      return;
    }

    await RemoveReactionFromMirroredMessage(
                                            client,
                                            ticket.ModMessageChannelId,
                                            messageEntity.MessageDiscordId,
                                            args.Emoji
                                           );
  }

  private static async Task ProcessTicketChannelReactionRemoval(
    DiscordClient client,
    IServiceScope scope,
    MessageReactionRemovedEventArgs args
  ) {
    Log.Debug(
              "[{Source}] Processing reaction removed in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
              nameof(OnMessageReactionRemovedEvent),
              args.Channel.Id,
              args.Message.Id,
              args.Emoji.Name
             );

    var topic = args.Channel.Topic;
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(topic);
    if (ticketId == Guid.Empty) {
      Log.Warning(
                  "[{Source}] Could not extract valid TicketId from channel topic. ChannelId: {ChannelId}, Topic: {Topic}",
                  nameof(OnMessageReactionRemovedEvent),
                  args.Channel.Id,
                  topic
                 );
      return;
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

    var messageEntity = await dbContext.TicketMessages.FirstOrDefaultAsync(x =>
                                                                             !x.SentByMod && x.BotMessageId == args.Message.Id && x.TicketId == ticketId);
    if (messageEntity is null) {
      Log.Debug(
                "[{Source}] No matching TicketMessage found for reaction removal in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
                nameof(OnMessageReactionRemovedEvent),
                args.Channel.Id,
                args.Message.Id,
                ticketId
               );
      return;
    }

    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x =>
                                                               !x.ClosedDateUtc.HasValue && x.Id == ticketId &&
                                                               x.ModMessageChannelId == args.Channel.Id);
    if (ticket is null) {
      Log.Warning(
                  "[{Source}] No active ticket found for reaction removal in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
                  nameof(OnMessageReactionRemovedEvent),
                  args.Channel.Id,
                  args.Message.Id,
                  ticketId
                 );
      return;
    }

    await RemoveReactionFromMirroredMessage(
                                            client,
                                            ticket.PrivateMessageChannelId,
                                            messageEntity.MessageDiscordId,
                                            args.Emoji
                                           );
  }

  private static async Task RemoveReactionFromMirroredMessage(
    DiscordClient client,
    ulong? channelId,
    ulong? messageId,
    DiscordEmoji emoji
  ) {
    if (!channelId.HasValue || !messageId.HasValue) {
      Log.Warning(
                  "[{Source}] Cannot remove reaction from mirrored message. Invalid ChannelId or MessageId. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
                  nameof(OnMessageReactionRemovedEvent),
                  channelId,
                  messageId,
                  emoji.Name
                 );
      return;
    }

    try {
      var channel = await client.GetChannelAsync(channelId.Value);
      var message = await channel.GetMessageAsync(messageId.Value);
      await message.DeleteOwnReactionAsync(emoji);
      Log.Information(
                      "[{Source}] Processed reaction removed {ChannelId} " +
                      "{MessageId} {MessageAuthor} {Emoji}",
                      nameof(OnMessageReactionRemovedEvent),
                      channel.Id,
                      message.Id,
                      message.Author?.Id,
                      emoji
                     );
    }
    catch (NotFoundException) {
      Log.Warning(
                  "[{Source}] Mirrored message not found, cannot remove reaction. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
                  nameof(OnMessageReactionRemovedEvent),
                  channelId,
                  messageId,
                  emoji.Name
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] An error occurred while removing the reaction from the mirrored message. ChannelId: {ChannelId}, MessageId: {MessageId}, Emoji: {Emoji}",
                nameof(OnMessageReactionRemovedEvent),
                channelId,
                messageId,
                emoji.Name
               );
    }
  }
}