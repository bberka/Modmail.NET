using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Queries;

namespace Modmail.NET.Features.Teams.Handlers;

public class GetTeamHandler : IRequestHandler<GetTeamQuery, GuildTeam>
{
  private readonly ModmailDbContext _dbContext;

  public GetTeamHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<GuildTeam> Handle(GetTeamQuery request, CancellationToken cancellationToken) {
    var team = await _dbContext.GuildTeams.FindAsync([request.Id], cancellationToken);
    if (!request.AllowNull && team is null) throw new TeamNotExistsException();

    return team;
  }
}