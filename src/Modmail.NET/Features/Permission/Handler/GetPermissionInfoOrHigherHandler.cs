using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Models;

namespace Modmail.NET.Features.Permission.Handler;

public class GetPermissionInfoOrHigherHandler : IRequestHandler<GetPermissionInfoOrHigherQuery, PermissionInfo[]>
{
  private readonly ModmailDbContext _dbContext;

  public GetPermissionInfoOrHigherHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<PermissionInfo[]> Handle(GetPermissionInfoOrHigherQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers
                           .Include(x => x.GuildTeam)
                           .Where(x => x.GuildTeam!.IsEnabled && x.GuildTeam.PermissionLevel >= request.LevelOrHigher)
                           .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
                           .ToArrayAsync(cancellationToken);
  }
}