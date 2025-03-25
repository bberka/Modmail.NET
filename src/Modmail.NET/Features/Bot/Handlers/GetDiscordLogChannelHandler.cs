using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MediatR;
using Modmail.NET.Features.Guild;
using Serilog;

namespace Modmail.NET.Features.Bot.Handlers;

public class GetDiscordLogChannelHandler : IRequestHandler<GetDiscordLogChannelQuery, DiscordChannel>
{
  private readonly ISender _sender;
  private readonly ModmailBot _bot;

  public GetDiscordLogChannelHandler(ISender sender,
                                     ModmailBot bot) {
    _sender = sender;
    _bot = bot;
  }

  public async Task<DiscordChannel> Handle(GetDiscordLogChannelQuery request, CancellationToken cancellationToken) {
    var guild = await _sender.Send(new GetDiscordMainGuildQuery(), cancellationToken);
    var option = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken) ?? throw new NullReferenceException();

    DiscordChannel logChannel;
    try {
      logChannel = await guild.GetChannelAsync(option.LogChannelId);
    }
    catch (NotFoundException) {
      logChannel = await _sender.Send(new ProcessCreateLogChannelCommand(_bot.Client.CurrentUser.Id, guild), cancellationToken);
      Log.Information("Log channel not found, created new log channel {LogChannelId}", logChannel.Id);
    }
    return logChannel;
  }
}