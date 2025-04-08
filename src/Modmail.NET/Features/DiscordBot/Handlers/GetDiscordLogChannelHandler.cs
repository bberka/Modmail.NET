using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MediatR;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Guild.Commands;
using Modmail.NET.Features.Guild.Queries;
using Serilog;

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