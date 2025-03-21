using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.UserInfo.Handlers;

public sealed class GetDiscordUserInfoHandler : IRequestHandler<GetDiscordUserInfoQuery, DiscordUserInfo>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public GetDiscordUserInfoHandler(ModmailDbContext dbContext,
                                   ModmailBot bot,
                                   ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task<DiscordUserInfo> Handle(GetDiscordUserInfoQuery request, CancellationToken cancellationToken) {
    if (request.UserId == 0) throw new InvalidUserIdException();
    var result = await _dbContext.DiscordUserInfos.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
    if (result is not null)
      return result;
    var discordUser = await _bot.Client.GetUserAsync(request.UserId);

    if (discordUser is not null) {
      await _sender.Send(new UpdateDiscordUserCommand(discordUser), cancellationToken);
      return result;
    }

    throw new NotFoundWithException(LangKeys.USER, request.UserId);
  }
}