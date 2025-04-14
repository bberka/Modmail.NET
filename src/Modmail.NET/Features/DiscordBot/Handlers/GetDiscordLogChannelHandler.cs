using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Guild.Commands;
using Modmail.NET.Features.Guild.Queries;
using Serilog;
using NotFoundException = DSharpPlus.Exceptions.NotFoundException;

namespace Modmail.NET.Features.DiscordBot.Handlers;

public class GetDiscordLogChannelHandler : IRequestHandler<GetDiscordLogChannelQuery, DiscordChannel>
{
  private readonly ModmailBot _bot;
  private readonly ISender _sender;

  public GetDiscordLogChannelHandler(ISender sender,
                                     ModmailBot bot) {
    _sender = sender;
    _bot = bot;
  }

  public async Task<DiscordChannel> Handle(GetDiscordLogChannelQuery request, CancellationToken cancellationToken) {
    var guild = await _sender.Send(new GetDiscordMainServerQuery(), cancellationToken);
    var option = await _sender.Send(new GetOptionQuery(), cancellationToken);

    if (option.LogChannelId == 0) return await _sender.Send(new ProcessCreateOrUpdateLogChannelCommand(_bot.Client.CurrentUser.Id, guild), cancellationToken);

    DiscordChannel logChannel;
    try {
      logChannel = await guild.GetChannelAsync(option.LogChannelId);
    }
    catch (NotFoundException) {
      logChannel = await _sender.Send(new ProcessCreateOrUpdateLogChannelCommand(_bot.Client.CurrentUser.Id, guild), cancellationToken);
      Log.Information("Log channel not found, created new log channel {LogChannelId}", logChannel.Id);
    }

    return logChannel;
  }
}