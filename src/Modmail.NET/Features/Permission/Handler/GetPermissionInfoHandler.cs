using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Models;

namespace Modmail.NET.Features.Permission.Handler;

public class GetPermissionInfoHandler : IRequestHandler<GetPermissionInfoQuery, List<PermissionInfo>>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public GetPermissionInfoHandler(ModmailBot bot,
                                      ModmailDbContext dbContext,
                                      ISender sender) {
    _bot = bot;
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<List<PermissionInfo>> Handle(GetPermissionInfoQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers
                           .Include(x => x.GuildTeam)
                           .Where(x => x.GuildTeam!.IsEnabled)
                           .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
                           .ToListAsync(cancellationToken);
  }
}