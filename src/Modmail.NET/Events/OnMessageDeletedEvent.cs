using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Aspects;
using Modmail.NET.Database;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Events;

public static class OnMessageDeletedEvent
{
  [PerformanceLoggerAspect]
  public static async Task OnMessageDeleted(
    DiscordClient client,
    MessageDeletedEventArgs args
  ) {
    _ = UserUpdateEvents.UpdateUser(client, args.Message.Author);

    if (args.Message.Author?.IsBot == true) {
      Log.Debug(
                "[{Source}] Ignoring message deletion from bot. MessageId: {MessageId}, ChannelId: {ChannelId}",
                nameof(OnMessageDeletedEvent),
                args.Message.Id,
                args.Channel.Id
               );
      return;
    }

    using var scope = client.ServiceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

    if (args.Channel.IsPrivate)
      await ProcessPrivateMessageDeletion(client, dbContext, args);
    else
      await ProcessTicketChannelMessageDeletion(client, dbContext, args);
  }

  private static async Task ProcessPrivateMessageDeletion(
    DiscordClient client,
    ModmailDbContext dbContext,
    MessageDeletedEventArgs args
  ) {
    Log.Debug(
              "[{Source}] Processing private message deletion. MessageId: {MessageId}, ChannelId: {ChannelId}",
              nameof(OnMessageDeletedEvent),
              args.Message.Id,
              args.Channel.Id
             );

    var messageEntity = await dbContext.TicketMessages
                                       .FirstOrDefaultAsync(
                                                            x => !x.SentByMod && x.MessageDiscordId == args.Message.Id
                                                           );
    if (messageEntity is null) {
      Log.Debug(
                "[{Source}] No TicketMessage entity found for deleted private message. MessageId: {MessageId}",
                nameof(OnMessageDeletedEvent),
                args.Message.Id
               );
      return;
    }

    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x =>
                                                               !x.ClosedDateUtc.HasValue && x.Id == messageEntity.TicketId &&
                                                               x.PrivateMessageChannelId == args.Channel.Id);
    if (ticket is null) {
      Log.Warning(
                  "[{Source}] No active ticket found for deleted private message. MessageId: {MessageId}, TicketId: {TicketId}",
                  nameof(OnMessageDeletedEvent),
                  args.Message.Id,
                  messageEntity.TicketId
                 );
      return;
    }

    await DeleteMirroredMessage(
                                client,
                                ticket.ModMessageChannelId,
                                messageEntity.BotMessageId
                               );
  }

  private static async Task ProcessTicketChannelMessageDeletion(
    DiscordClient client,
    ModmailDbContext dbContext,
    MessageDeletedEventArgs args
  ) {
    Log.Debug(
              "[{Source}] Processing ticket channel message deletion. MessageId: {MessageId}, ChannelId: {ChannelId}",
              nameof(OnMessageDeletedEvent),
              args.Message.Id,
              args.Channel.Id
             );

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(args.Channel.Topic);
    if (ticketId == Guid.Empty) {
      Log.Warning(
                  "[{Source}] Could not extract valid TicketId from channel topic. ChannelId: {ChannelId}, Topic: {Topic}",
                  nameof(OnMessageDeletedEvent),
                  args.Channel.Id,
                  args.Channel.Topic
                 );
      return;
    }

    var messageEntity = await dbContext.TicketMessages.FirstOrDefaultAsync(x =>
                                                                             x.SentByMod && x.TicketId == ticketId && x.MessageDiscordId == args.Message.Id);
    if (messageEntity is null) {
      Log.Debug(
                "[{Source}] No TicketMessage entity found for deleted ticket channel message. MessageId: {MessageId}, TicketId: {TicketId}",
                nameof(OnMessageDeletedEvent),
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
                  "[{Source}] No active ticket found for deleted ticket channel message. MessageId: {MessageId}, TicketId: {TicketId}",
                  nameof(OnMessageDeletedEvent),
                  args.Message.Id,
                  ticketId
                 );
      return;
    }

    await DeleteMirroredMessage(
                                client,
                                ticket.PrivateMessageChannelId,
                                messageEntity.BotMessageId
                               );
  }

  private static async Task DeleteMirroredMessage(
    DiscordClient client,
    ulong? channelId,
    ulong? botMessageId
  ) {
    if (!channelId.HasValue || !botMessageId.HasValue) {
      Log.Warning(
                  "[{Source}] Cannot delete mirrored message. Invalid ChannelId or BotMessageId. ChannelId: {ChannelId}, BotMessageId: {BotMessageId}",
                  nameof(OnMessageDeletedEvent),
                  channelId,
                  botMessageId
                 );
      return;
    }

    try {
      var channel = await client.GetChannelAsync(channelId.Value);
      var message = await channel.GetMessageAsync(botMessageId.Value);
      await message.DeleteAsync();
      Log.Information(
                      "[{Source}] Processed message deleted {ChannelId} {MessageId} " +
                      "{MessageAuthor}",
                      nameof(OnMessageDeletedEvent),
                      channel.Id,
                      message.Id,
                      message.Author?.Id
                     );
    }
    catch (NotFoundException) {
      Log.Warning(
                  "[{Source}] Mirrored message not found, assuming it was already deleted. ChannelId: {ChannelId}, BotMessageId: {BotMessageId}",
                  nameof(OnMessageDeletedEvent),
                  channelId,
                  botMessageId
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] An error occurred while deleting the mirrored message. ChannelId: {ChannelId}, BotMessageId: {BotMessageId}",
                nameof(OnMessageDeletedEvent),
                channelId,
                botMessageId
               );
    }
  }
}