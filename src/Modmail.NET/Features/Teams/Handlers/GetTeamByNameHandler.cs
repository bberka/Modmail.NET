using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

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
    if (!request.AllowNull && result is null) throw new NotFoundWithException(LangKeys.Team, request.Name);
    return result;
  }
}