using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MediatR;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Bot.Handlers;

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
    foreach (var guild in _bot.Client.Guilds)
      try {
        var member = await guild.Value.GetMemberAsync(request.UserId);
        await _sender.Send(new UpdateDiscordUserCommand(member), cancellationToken);
        return member;
      }
      catch (NotFoundException) {
        continue;
      }
      catch (Exception) {
        // ignored
      }

    return null;
  }
}