using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Blacklist.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class GetAllBlacklistHandler : IRequestHandler<GetAllBlacklistQuery, List<TicketBlacklist>>
{
  private readonly ModmailDbContext _dbContext;

  public GetAllBlacklistHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<TicketBlacklist>> Handle(GetAllBlacklistQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.TicketBlacklists.ToListAsync(cancellationToken);
    if (result.Count == 0) throw new EmptyListResultException(LangKeys.BlacklistedUsers);
    return result;
  }
}