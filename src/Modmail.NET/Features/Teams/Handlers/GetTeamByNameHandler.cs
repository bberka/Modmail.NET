using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public class GetTeamByNameHandler : IRequestHandler<GetTeamByNameQuery, GuildTeam>
{
  private readonly ModmailDbContext _dbContext;

  public GetTeamByNameHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<GuildTeam> Handle(GetTeamByNameQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.GuildTeams
                                 .FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);
    if (!request.AllowNull && result is null) throw new NotFoundWithException(LangKeys.TEAM, request.Name);
    return result;
  }
}