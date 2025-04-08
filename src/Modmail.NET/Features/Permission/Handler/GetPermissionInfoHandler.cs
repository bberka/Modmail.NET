using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Permission.Models;
using Modmail.NET.Features.Permission.Queries;

namespace Modmail.NET.Features.Permission.Handler;

public class GetPermissionInfoHandler : IRequestHandler<GetPermissionInfoQuery, PermissionInfo[]>
{
  private readonly ModmailDbContext _dbContext;

  public GetPermissionInfoHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<PermissionInfo[]> Handle(GetPermissionInfoQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers
                           .Include(x => x.GuildTeam)
                           .Where(x => x.GuildTeam!.IsEnabled)
                           .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
                           .ToArrayAsync(cancellationToken);
  }
}