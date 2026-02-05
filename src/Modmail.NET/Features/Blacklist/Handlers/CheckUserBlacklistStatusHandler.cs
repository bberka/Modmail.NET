using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Blacklist.Queries;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class CheckUserBlacklistStatusHandler : IRequestHandler<CheckUserBlacklistStatusQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckUserBlacklistStatusHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckUserBlacklistStatusQuery request, CancellationToken cancellationToken) {
    if (request.DiscordUserId <= 0) throw new InvalidUserIdException();
    return await _dbContext.TicketBlacklists.AnyAsync(x => x.DiscordUserId == request.DiscordUserId, cancellationToken);
  }
}