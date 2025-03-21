using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Models;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class GetTeamPermissionInfoHandler : IRequestHandler<GetTeamPermissionInfoQuery, List<PermissionInfo>>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public GetTeamPermissionInfoHandler(ModmailBot bot,
                                      ModmailDbContext dbContext,
                                      ISender sender) {
    _bot = bot;
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<List<PermissionInfo>> Handle(GetTeamPermissionInfoQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildTeamMembers
                           .Include(x => x.GuildTeam)
                           .Where(x => x.GuildTeam!.IsEnabled)
                           .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
                           .ToListAsync(cancellationToken);
  }
}