using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Teams.Handlers;

public class CheckUserInAnyTeamHandler : IRequestHandler<CheckUserInAnyTeamQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckUserInAnyTeamHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckUserInAnyTeamQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers.AnyAsync(x => x.Key == request.MemberId && x.Type == TeamMemberDataType.UserId, cancellationToken);
  }
}