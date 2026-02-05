using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MediatR;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.User.Commands;

namespace Modmail.NET.Features.DiscordBot.Handlers;

public class GetDiscordMemberHandler : IRequestHandler<GetDiscordMemberQuery, DiscordMember>
{
  private readonly ModmailBot _bot;
  private readonly ISender _sender;

  public GetDiscordMemberHandler(ModmailBot bot,
                                 ISender sender) {
    _bot = bot;
    _sender = sender;
  }

  public async Task<DiscordMember> Handle(GetDiscordMemberQuery request, CancellationToken cancellationToken) {
    foreach (var guild in _bot.Client.Guilds) {
      try {
        var member = await guild.Value.GetMemberAsync(request.UserId);
        await _sender.Send(new UpdateDiscordUserCommand(member), cancellationToken);
        return member;
      }
      catch (NotFoundException) {
        // ignored
      }
      catch (Exception) {
        // ignored
      }
    }

    return null;
  }
}