using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Blacklist.Queries;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class GetBlacklistHandler : IRequestHandler<GetBlacklistQuery, TicketBlacklist>
{
  private readonly ModmailDbContext _dbContext;

  public GetBlacklistHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<TicketBlacklist> Handle(GetBlacklistQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.TicketBlacklists.FirstOrDefaultAsync(x => x.DiscordUserId == request.DiscordUserId, cancellationToken);
    if (result is null && !request.AllowNull) throw new UserIsNotBlacklistedException();
    return result;
  }
}