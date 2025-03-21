using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class GetTeamListHandler : IRequestHandler<GetTeamListQuery, List<GuildTeam>>
{
  private readonly ModmailDbContext _dbContext;

  public GetTeamListHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<GuildTeam>> Handle(GetTeamListQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.GuildTeams
                                 .Include(x => x.GuildTeamMembers)
                                 .ToListAsync(cancellationToken);

    if (request.ThrowIfEmpty && result.Count == 0) throw new EmptyListResultException(LangKeys.TEAM);

    return result;
  }
}