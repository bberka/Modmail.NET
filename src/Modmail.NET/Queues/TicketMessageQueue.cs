using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Abstract;
using Modmail.NET.Aspects;
using Modmail.NET.Features.Blacklist;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Queues;

public class TicketMessageQueue : BaseQueue<ulong, MessageCreatedEventArgs>
{
  private readonly IOptions<BotConfig> _options;
  private readonly IServiceScopeFactory _scopeFactory;

  public TicketMessageQueue(
    IServiceScopeFactory scopeFactory,
    IOptions<BotConfig> options
  ) : base(TimeSpan.FromMinutes(15)) {
    _scopeFactory = scopeFactory;
    _options = options;
  }

  protected override async Task Handle(ulong userId, MessageCreatedEventArgs args) {
    if (args.Message.Content.StartsWith(_options.Value.BotPrefix)) {
      Log.Debug(
                "[{Source}] Ignoring message due to bot prefix. UserId: {UserId}, Message: {Message}",
                nameof(TicketMessageQueue),
                userId,
                args.Message.Content
               );
      return;
    }

    if (args.Channel.IsPrivate)
      await HandlePrivateTicketMessageAsync(args.Message, args.Channel, args.Author);
    else
      await HandleGuildTicketMessageAsync(args.Message, args.Channel, args.Author, args.Guild);
  }

  [PerformanceLoggerAspect]
  private async Task HandlePrivateTicketMessageAsync(
    DiscordMessage message,
    DiscordChannel channel,
    DiscordUser user
  ) {
    Log.Debug(
              "[{Source}] Handling private ticket message. UserId: {UserId}, Message: {Message}",
              nameof(TicketMessageQueue),
              user.Id,
              message.Content
             );

    using var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    try {
      if (await sender.Send(new CheckUserBlacklistStatusQuery(user.Id))) {
        Log.Information(
                        "[{Source}] User is blacklisted, sending rejection message. UserId: {UserId}",
                        nameof(TicketMessageQueue),
                        user.Id
                       );
        await channel.SendMessageAsync(UserResponses.YouHaveBeenBlacklisted());
        return;
      }

      var activeTicket = await sender.Send(new GetTicketByUserIdQuery(user.Id, true, true));
      if (activeTicket is not null) {
        Log.Debug(
                  "[{Source}] Active ticket found, processing user message. TicketId: {TicketId}, UserId: {UserId}",
                  nameof(TicketMessageQueue),
                  activeTicket.Id,
                  user.Id
                 );
        await sender.Send(new ProcessUserSentMessageCommand(activeTicket.Id, message, channel));
      }
      else {
        Log.Debug(
                  "[{Source}] No active ticket found, creating a new ticket. UserId: {UserId}",
                  nameof(TicketMessageQueue),
                  user.Id
                 );
        await sender.Send(new ProcessCreateNewTicketCommand(user, channel, message));
      }

      Log.Information(
                      "[{Source}] Processed private message. UserId: {UserId}, Message: {Message}",
                      nameof(TicketMessageQueue),
                      user.Id,
                      message.Content
                     );
    }
    catch (BotExceptionBase ex) {
      Log.Warning(
                  ex,
                  "[{Source}] BotExceptionBase: Error processing private message. UserId: {UserId}, Message: {Message}",
                  nameof(TicketMessageQueue),
                  user.Id,
                  message.Content
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] Unexpected error processing private message. UserId: {UserId}, Message: {Message}",
                nameof(TicketMessageQueue),
                user.Id,
                message.Content
               );
    }
  }

  [PerformanceLoggerAspect]
  private async Task HandleGuildTicketMessageAsync(
    DiscordMessage message,
    DiscordChannel channel,
    DiscordUser modUser,
    DiscordGuild guild
  ) {
    Log.Debug(
              "[{Source}] Handling guild ticket message. ChannelId: {ChannelId}, UserId: {UserId}, Message: {Message}",
              nameof(TicketMessageQueue),
              channel.Id,
              modUser.Id,
              message.Content
             );

    using var scope = _scopeFactory.CreateScope();
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
    if (ticketId == Guid.Empty) {
      Log.Warning(
                  "[{Source}] Invalid ticket id, ignoring message. ChannelId: {ChannelId}, Topic: {Topic}",
                  nameof(TicketMessageQueue),
                  channel.Id,
                  channel.Topic
                 );
      return;
    }

    try {
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      await sender.Send(new ProcessModSendMessageCommand(ticketId, modUser, message, channel, guild));
      Log.Information(
                      "[{Source}] Processed guild message. TicketId: {TicketId}, UserId: {UserId}, Message: {Message}",
                      nameof(TicketMessageQueue),
                      ticketId,
                      modUser.Id,
                      message.Content
                     );
    }
    catch (BotExceptionBase ex) {
      Log.Warning(
                  ex,
                  "[{Source}] BotExceptionBase: Error processing guild message. TicketId: {TicketId}, UserId: {UserId}, Message: {Message}",
                  nameof(TicketMessageQueue),
                  ticketId,
                  modUser.Id,
                  message.Content
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] Unexpected error processing guild message. TicketId: {TicketId}, UserId: {UserId}, Message: {Message}",
                nameof(TicketMessageQueue),
                ticketId,
                modUser.Id,
                message.Content
               );
    }
  }
}