using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Permission.Handler;

public class CheckRoleInAnyTeamHandler : IRequestHandler<CheckRoleInAnyTeamQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckRoleInAnyTeamHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckRoleInAnyTeamQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers.AnyAsync(x => x.Key == request.RoleId && x.Type == TeamMemberDataType.RoleId, cancellationToken);
  }
}