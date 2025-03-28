using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Models;

namespace Modmail.NET.Features.Permission.Handler;

public class GetPermissionInfoHandler : IRequestHandler<GetPermissionInfoQuery, List<PermissionInfo>>
{
  private readonly ModmailDbContext _dbContext;

  public GetPermissionInfoHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<PermissionInfo>> Handle(GetPermissionInfoQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers
                           .Include(x => x.GuildTeam)
                           .Where(x => x.GuildTeam!.IsEnabled)
                           .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
                           .ToListAsync(cancellationToken);
  }
}