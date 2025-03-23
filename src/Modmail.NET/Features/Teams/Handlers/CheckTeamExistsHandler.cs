using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Teams.Handlers;

public class CheckTeamExistsHandler : IRequestHandler<CheckTeamExistsQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckTeamExistsHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckTeamExistsQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeams.AnyAsync(x => x.Name == request.Name, cancellationToken);
  }
}