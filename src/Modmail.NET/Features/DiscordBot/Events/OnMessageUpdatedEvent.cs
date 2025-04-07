using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.Guild.Queries;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.User.Commands;
using Serilog;

namespace Modmail.NET.Features.DiscordBot.Events;

public static class OnMessageUpdatedEvent
{
  [PerformanceLoggerAspect]
  public static async Task OnMessageUpdated(
    DiscordClient client,
    MessageUpdatedEventArgs args
  ) {
    if (args is null) {
      Log.Debug(
                "[{Source}] MessageUpdatedEventArgs is null, exiting",
                nameof(OnMessageUpdatedEvent)
               );
      return;
    }

    if (args.Author.IsBot) {
      Log.Debug(
                "[{Source}] Ignoring message update from bot. UserId: {UserId}, MessageId: {MessageId}",
                nameof(OnMessageUpdatedEvent),
                args.Author.Id,
                args.Message.Id
               );
      return;
    }

    if (args.MessageBefore?.Content == args.Message.Content) {
      Log.Debug(
                "[{Source}] Message content unchanged, ignoring update. MessageId: {MessageId}",
                nameof(OnMessageUpdatedEvent),
                args.Message.Id
               );
      return;
    }

    using var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Author));

    if (args.Channel.IsPrivate)
      await ProcessPrivateChannelMessageUpdate(client, scope, args);
    else
      await ProcessTicketChannelMessageUpdate(client, scope, args);
  }

  private static async Task ProcessPrivateChannelMessageUpdate(
    DiscordClient client,
    IServiceScope scope,
    MessageUpdatedEventArgs args
  ) {
    Log.Debug(
              "[{Source}] Processing message update in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
              nameof(OnMessageUpdatedEvent),
              args.Channel.Id,
              args.Message.Id
             );

    if (args.Message.Author?.Id == client.CurrentUser.Id) {
      Log.Debug(
                "[{Source}] Ignoring message update on bot's own message in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
                nameof(OnMessageUpdatedEvent),
                args.Channel.Id,
                args.Message.Id
               );
      return;
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
    var messageEntity = await dbContext.TicketMessages
                                       .FirstOrDefaultAsync(x =>
                                                              !x.SentByMod && x.MessageDiscordId == args.Message.Id &&
                                                              x.SenderUserId == args.Message.Author.Id);
    if (messageEntity is null) {
      Log.Debug(
                "[{Source}] No matching TicketMessage found for update in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
                nameof(OnMessageUpdatedEvent),
                args.Channel.Id,
                args.Message.Id
               );
      return;
    }

    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x =>
                                                               !x.ClosedDateUtc.HasValue && x.Id == messageEntity.TicketId &&
                                                               x.PrivateMessageChannelId == args.Channel.Id);
    if (ticket is null) {
      Log.Warning(
                  "[{Source}] No active ticket found for update in private channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
                  nameof(OnMessageUpdatedEvent),
                  args.Channel.Id,
                  args.Message.Id,
                  messageEntity.TicketId
                 );
      return;
    }

    await UpdateMirroredMessage(
                                client,
                                scope,
                                ticket.ModMessageChannelId,
                                messageEntity.BotMessageId,
                                args.MessageBefore,
                                args.Message,
                                ticket.Anonymous
                               );
  }

  private static async Task ProcessTicketChannelMessageUpdate(
    DiscordClient client,
    IServiceScope scope,
    MessageUpdatedEventArgs args
  ) {
    Log.Debug(
              "[{Source}] Processing message update in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}",
              nameof(OnMessageUpdatedEvent),
              args.Channel.Id,
              args.Message.Id
             );

    var topic = args.Channel.Topic;
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(topic);
    if (ticketId == Guid.Empty) {
      Log.Warning(
                  "[{Source}] Could not extract valid TicketId from channel topic. ChannelId: {ChannelId}, Topic: {Topic}",
                  nameof(OnMessageUpdatedEvent),
                  args.Channel.Id,
                  topic
                 );
      return;
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

    var messageEntity = await dbContext.TicketMessages.FirstOrDefaultAsync(x =>
                                                                             x.SentByMod && x.MessageDiscordId == args.Message.Id &&
                                                                             x.SenderUserId == args.Message.Author.Id);
    if (messageEntity is null) {
      Log.Debug(
                "[{Source}] No matching TicketMessage found for update in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
                nameof(OnMessageUpdatedEvent),
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
                  "[{Source}] No active ticket found for update in ticket channel. ChannelId: {ChannelId}, MessageId: {MessageId}, TicketId: {TicketId}",
                  nameof(OnMessageUpdatedEvent),
                  args.Channel.Id,
                  args.Message.Id,
                  ticketId
                 );
      return;
    }

    await UpdateMirroredMessage(
                                client,
                                scope,
                                ticket.PrivateMessageChannelId,
                                messageEntity.BotMessageId,
                                args.MessageBefore,
                                args.Message,
                                ticket.Anonymous
                               );
  }

  private static async Task UpdateMirroredMessage(
    DiscordClient client,
    IServiceScope scope,
    ulong channelId,
    ulong messageId,
    DiscordMessage oldMessage,
    DiscordMessage updatedMessage,
    bool anonymous
  ) {
    try {
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      var option = await sender.Send(new GetGuildOptionQuery(false));

      var channel = await client.GetChannelAsync(channelId);
      var message = await channel.GetMessageAsync(messageId);
      var embed =
        updatedMessage.Channel!.IsPrivate
          ? TicketBotMessages.Ticket.MessageEdited(updatedMessage)
          : TicketBotMessages.User.MessageEdited(updatedMessage, option.AlwaysAnonymous || anonymous);

      //TODO: Add support for removing attachment files from message on message update event
      //Currently the library does not support removing single attachments, either we have to remove all of them 
      //Or only remove if new message has zero attachemnts but if it went from 4 to 3 then we do nothing 
      //The only workaround for this is to upload remaining files to discord again which is unnecessary work
      await message.ModifyAsync(x => {
        x.ClearEmbeds();
        x.AddEmbed(embed);
      });

      Log.Information(
                      "[{Source}] Processed message edited {ChannelId} " +
                      "{MessageId} {MessageAuthor}",
                      nameof(OnMessageUpdatedEvent),
                      channel.Id,
                      message.Id,
                      updatedMessage.Author?.Id
                     );
    }
    catch (NotFoundException) {
      Log.Warning(
                  "[{Source}] Mirrored message not found, cannot update. ChannelId: {ChannelId}, MessageId: {MessageId}",
                  nameof(OnMessageUpdatedEvent),
                  channelId,
                  messageId
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] An error occurred while updating the mirrored message. ChannelId: {ChannelId}, MessageId: {MessageId}",
                nameof(OnMessageUpdatedEvent),
                channelId,
                messageId
               );
    }
  }
}