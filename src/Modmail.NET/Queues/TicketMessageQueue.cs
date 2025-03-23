using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Abstract;
using Modmail.NET.Aspects;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Blacklist;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Models.Dto;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Queues;

public sealed class TicketMessageQueue : BaseQueue<ulong, DiscordTicketMessageDto>
{
  private readonly IOptions<BotConfig> _options;
  private readonly IServiceScopeFactory _scopeFactory;

  public TicketMessageQueue(IServiceScopeFactory scopeFactory,
                            IOptions<BotConfig> options) : base(TimeSpan.FromMinutes(15)) {
    _scopeFactory = scopeFactory;
    _options = options;
  }

  protected override async Task HandleMessageAsync(ulong userId, DiscordTicketMessageDto dto) {
    if (dto.Args.Channel.IsPrivate)
      await HandlePrivateTicketMessageAsync(dto.Sender, dto.Args.Message, dto.Args.Channel, dto.Args.Author);
    else
      await HandleGuildTicketMessageAsync(dto.Sender, dto.Args.Message, dto.Args.Channel, dto.Args.Author, dto.Args.Guild);
  }

  [PerformanceLoggerAspect]
  private async Task HandlePrivateTicketMessageAsync(DiscordClient client, DiscordMessage message, DiscordChannel channel, DiscordUser user) {
    if (message.Content.StartsWith(_options.Value.BotPrefix))
      return;
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var bot = scope.ServiceProvider.GetRequiredService<ModmailBot>();

    try {
      if (await sender.Send(new CheckUserBlacklistStatusQuery(bot.Client.CurrentUser.Id,user.Id))) {
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
  private async Task HandleGuildTicketMessageAsync(DiscordClient client, DiscordMessage message, DiscordChannel channel, DiscordUser modUser, DiscordGuild guild) {
    if (message.Content.StartsWith(_options.Value.BotPrefix))
      return;

    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();


    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channel.Topic);
      if (ticketId == Guid.Empty) return;

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