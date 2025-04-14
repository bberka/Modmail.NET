using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Blacklist.Static;
using Modmail.NET.Features.Ticket.Commands;
using Serilog;

namespace Modmail.NET.Features.Ticket.Services;

public class TicketMessage : MemoryQueueBase<ulong, MessageCreatedEventArgs>
{
  private readonly IOptions<BotConfig> _options;
  private readonly IServiceScopeFactory _scopeFactory;

  public TicketMessage(
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
                nameof(TicketMessage),
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
              nameof(TicketMessage),
              user.Id,
              message.Content
             );

    using var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

    try {
      if (await dbContext.Blacklists.FilterByUserId(user.Id).AnyAsync()) {
        Log.Information(
                        "[{Source}] User is blacklisted, sending rejection message. UserId: {UserId}",
                        nameof(TicketMessage),
                        user.Id
                       );
        await channel.SendMessageAsync(BlacklistBotMessages.YouHaveBeenBlacklisted());
        return;
      }


      var activeTicket = await dbContext.Tickets
                                        .FilterActive()
                                        .FilterByOpenerUserId(user.Id)
                                        .FirstOrDefaultAsync();

      if (activeTicket is not null) {
        Log.Debug(
                  "[{Source}] Active ticket found, processing user message. TicketId: {TicketId}, UserId: {UserId}",
                  nameof(TicketMessage),
                  activeTicket.Id,
                  user.Id
                 );
        await sender.Send(new ProcessUserSentMessageCommand(activeTicket.Id, message, channel));
      }
      else {
        Log.Debug(
                  "[{Source}] No active ticket found, creating a new ticket. UserId: {UserId}",
                  nameof(TicketMessage),
                  user.Id
                 );
        await sender.Send(new ProcessCreateNewTicketCommand(user, channel, message));
      }

      Log.Information(
                      "[{Source}] Processed private message. UserId: {UserId}, Message: {Message}",
                      nameof(TicketMessage),
                      user.Id,
                      message.Content
                     );
    }
    catch (ModmailBotException ex) {
      Log.Warning(
                  ex,
                  "[{Source}] ModmailBotException: Error processing private message. UserId: {UserId}, Message: {Message}",
                  nameof(TicketMessage),
                  user.Id,
                  message.Content
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] Unexpected error processing private message. UserId: {UserId}, Message: {Message}",
                nameof(TicketMessage),
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
              nameof(TicketMessage),
              channel.Id,
              modUser.Id,
              message.Content
             );

    using var scope = _scopeFactory.CreateScope();
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
    if (ticketId == Guid.Empty) {
      Log.Warning(
                  "[{Source}] Invalid ticket id, ignoring message. ChannelId: {ChannelId}, Topic: {Topic}",
                  nameof(TicketMessage),
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
                      nameof(TicketMessage),
                      ticketId,
                      modUser.Id,
                      message.Content
                     );
    }
    catch (ModmailBotException ex) {
      Log.Warning(
                  ex,
                  "[{Source}] ModmailBotException: Error processing guild message. TicketId: {TicketId}, UserId: {UserId}, Message: {Message}",
                  nameof(TicketMessage),
                  ticketId,
                  modUser.Id,
                  message.Content
                 );
    }
    catch (Exception ex) {
      Log.Error(
                ex,
                "[{Source}] Unexpected error processing guild message. TicketId: {TicketId}, UserId: {UserId}, Message: {Message}",
                nameof(TicketMessage),
                ticketId,
                modUser.Id,
                message.Content
               );
    }
  }
}