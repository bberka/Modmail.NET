using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class GetTeamPermissionLevelHandler : IRequestHandler<GetTeamPermissionLevelQuery, TeamPermissionLevel?>
{
  private readonly ModmailBot _bot;
  private readonly IOptions<BotConfig> _options;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public GetTeamPermissionLevelHandler(ModmailDbContext dbContext,
                                       ModmailBot bot,
                                       IOptions<BotConfig> options,
                                       ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _options = options;
    _sender = sender;
  }

  public async Task<TeamPermissionLevel?> Handle(GetTeamPermissionLevelQuery request, CancellationToken cancellationToken) {
    if (_options.Value.OwnerUsers.Contains(request.UserId)) {
      return TeamPermissionLevel.Owner;
    }
    var teamMember = await _dbContext.GuildTeamMembers
                                     .Include(x => x.GuildTeam)
                                     .Where(x => (x.Type == TeamMemberDataType.RoleId && request.RoleIdList.Contains(x.Key)) || (x.Key == request.UserId && x.Type == TeamMemberDataType.UserId))
                                     .OrderByDescending(x => x.GuildTeam!.PermissionLevel)
                                     .FirstOrDefaultAsync(cancellationToken);
    return teamMember?.GuildTeam?.PermissionLevel;
  }
}