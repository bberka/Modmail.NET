using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class GetTeamHandler : IRequestHandler<GetTeamQuery, GuildTeam>
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