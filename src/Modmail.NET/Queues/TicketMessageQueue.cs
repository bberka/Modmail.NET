using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Abstract;
using Modmail.NET.Aspects;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Models.Dto;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Queues;

public sealed class TicketMessageQueue : BaseQueue<ulong, DiscordTicketMessageDto>
{
  private static TicketMessageQueue? _instance;
  public static TicketMessageQueue This => _instance ??= new TicketMessageQueue();
  private TicketMessageQueue() : base(TimeSpan.FromMinutes(15)) { }

  protected override async Task HandleMessageAsync(ulong userId, DiscordTicketMessageDto dto) {
    if (dto.Args.Channel.IsPrivate)
      await HandlePrivateTicketMessageAsync(dto.Sender, dto.Args.Message, dto.Args.Channel, dto.Args.Author);
    else
      await HandleGuildTicketMessageAsync(dto.Sender, dto.Args.Message, dto.Args.Channel, dto.Args.Author, dto.Args.Guild);
  }

  [PerformanceLoggerAspect]
  private async Task HandlePrivateTicketMessageAsync(DiscordClient sender, DiscordMessage message, DiscordChannel channel, DiscordUser user) {
    if (message.Content.StartsWith(BotConfig.This.BotPrefix))
      return;

    try {
      if (await TicketBlacklist.IsBlacklistedAsync(user.Id)) {
        await channel.SendMessageAsync(UserResponses.YouHaveBeenBlacklisted());
        return;
      }

      var activeTicket = await Ticket.GetActiveTicketNullableAsync(user.Id);
      if (activeTicket is not null)
        await activeTicket.ProcessUserSentMessageAsync(message, channel);
      else
        await Ticket.ProcessCreateNewTicketAsync(user, channel, message);

      Log.Information("[TicketMessageQueue] Processed private message from {UserId}: {Message}", user.Id, message.Content);
    }
    catch (BotExceptionBase ex) {
      Log.Warning(ex, "[TicketMessageQueue] Error processing private message from {UserId}: {Message}", user.Id, message.Content);
    }
    catch (Exception ex) {
      Log.Error(ex, "[TicketMessageQueue] Unexpected error processing private message from {UserId}: {Message}", user.Id, message.Content);
    }
  }

  [PerformanceLoggerAspect]
  private async Task HandleGuildTicketMessageAsync(DiscordClient sender, DiscordMessage message, DiscordChannel channel, DiscordUser modUser, DiscordGuild guild) {
    if (message.Content.StartsWith(BotConfig.This.BotPrefix))
      return;

    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
      if (ticketId == Guid.Empty) return;

      var ticket = await Ticket.GetActiveTicketAsync(ticketId);
      await ticket.ProcessModSendMessageAsync(modUser, message, channel, guild);

      Log.Information("[TicketMessageQueue] Processed guild message from {UserId}: {Message}", modUser.Id, message.Content);
    }
    catch (BotExceptionBase ex) {
      Log.Warning(ex, "[TicketMessageQueue] Error processing guild message from {UserId}: {Message}", modUser.Id, message.Content);
    }
    catch (Exception ex) {
      Log.Error(ex, "[TicketMessageQueue] Unexpected error processing guild message from {UserId}: {Message}", modUser.Id, message.Content);
    }
  }
}