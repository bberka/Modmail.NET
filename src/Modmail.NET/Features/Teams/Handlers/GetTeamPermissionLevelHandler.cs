using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class GetTeamPermissionLevelHandler : IRequestHandler<GetTeamPermissionLevelQuery, TeamPermissionLevel?>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public GetTeamPermissionLevelHandler(ModmailDbContext dbContext,
                                       ModmailBot bot,
                                       ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task<TeamPermissionLevel?> Handle(GetTeamPermissionLevelQuery request, CancellationToken cancellationToken) {
    var teamMember = await _dbContext.GuildTeamMembers
                                     .Include(x => x.GuildTeam)
                                     .Where(x => (x.Type == TeamMemberDataType.RoleId && request.RoleIdList.Contains(x.Key)) || (x.Key == request.UserId && x.Type == TeamMemberDataType.UserId))
                                     .OrderByDescending(x => x.GuildTeam!.PermissionLevel)
                                     .FirstOrDefaultAsync(cancellationToken);
    return teamMember?.GuildTeam!.PermissionLevel;
  }
}