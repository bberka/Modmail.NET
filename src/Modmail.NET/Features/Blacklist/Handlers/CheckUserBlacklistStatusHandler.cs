using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Blacklist.Handlers;

public sealed class CheckUserBlacklistStatusHandler : IRequestHandler<CheckUserBlacklistStatusQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckUserBlacklistStatusHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckUserBlacklistStatusQuery request, CancellationToken cancellationToken) {
    if (request.DiscordUserId == 0) throw new ArgumentException();
    return await _dbContext.TicketBlacklists.AnyAsync(x => x.DiscordUserId == request.DiscordUserId, cancellationToken);
  }
}