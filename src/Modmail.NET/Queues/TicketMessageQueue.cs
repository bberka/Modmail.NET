using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Abstract;
using Modmail.NET.Aspects;
using Modmail.NET.Features.Blacklist;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Models.Dto;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Queues;

public class TicketMessageQueue : BaseQueue<ulong, DiscordTicketMessageDto>
{
  private readonly IOptions<BotConfig> _options;
  private readonly IServiceScopeFactory _scopeFactory;

  public TicketMessageQueue(IServiceScopeFactory scopeFactory,
                            IOptions<BotConfig> options) : base(TimeSpan.FromMinutes(15)) {
    _scopeFactory = scopeFactory;
    _options = options;
  }

  protected override async Task Handle(ulong userId, DiscordTicketMessageDto dto) {
    if (dto.Args.Message.Content.StartsWith(_options.Value.BotPrefix))
      return;

    if (dto.Args.Channel.IsPrivate)
      await HandlePrivateTicketMessageAsync(dto.Args.Message, dto.Args.Channel, dto.Args.Author);
    else
      await HandleGuildTicketMessageAsync(dto.Args.Message, dto.Args.Channel, dto.Args.Author, dto.Args.Guild);
  }

  [PerformanceLoggerAspect]
  private async Task HandlePrivateTicketMessageAsync(DiscordMessage message, DiscordChannel channel, DiscordUser user) {
    using var scope = _scopeFactory.CreateScope();
    try {
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      if (await sender.Send(new CheckUserBlacklistStatusQuery(user.Id))) {
        await channel.SendMessageAsync(UserResponses.YouHaveBeenBlacklisted());
        return;
      }

      var activeTicket = await sender.Send(new GetTicketByUserIdQuery(user.Id, true, true));
      if (activeTicket is not null)
        await sender.Send(new ProcessUserSentMessageCommand(activeTicket.Id, message, channel));
      else
        await sender.Send(new ProcessCreateNewTicketCommand(user, channel, message));

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
  private async Task HandleGuildTicketMessageAsync(DiscordMessage message, DiscordChannel channel, DiscordUser modUser, DiscordGuild guild) {
    using var scope = _scopeFactory.CreateScope();
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
      if (ticketId == Guid.Empty) return;

      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      await sender.Send(new ProcessModSendMessageCommand(ticketId, modUser, message, channel, guild));
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