using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;
using NotFoundException = DSharpPlus.Exceptions.NotFoundException;

namespace Modmail.NET.Features.User.Handlers;

public class GetDiscordUserInfoHandler : IRequestHandler<GetDiscordUserInfoQuery, DiscordUserInfo>
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

    try {
      var discordUser = await _bot.Client.GetUserAsync(request.UserId);
      await _sender.Send(new UpdateDiscordUserCommand(discordUser), cancellationToken);
      result = await _dbContext.DiscordUserInfos.SingleAsync(x => x.Id == request.UserId, cancellationToken);
      return result;
    }
    catch (NotFoundException) {
      //ignored
    }

    throw new NotFoundWithException(LangKeys.User, request.UserId);
  }
}