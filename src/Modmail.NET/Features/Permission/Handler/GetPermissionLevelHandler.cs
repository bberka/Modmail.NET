using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Features.Permission.Static;
using Modmail.NET.Features.Teams.Static;

namespace Modmail.NET.Features.Permission.Handler;

public class GetPermissionLevelHandler : IRequestHandler<GetPermissionLevelQuery, TeamPermissionLevel?>
{
  private readonly ModmailDbContext _dbContext;
  private readonly IOptions<BotConfig> _options;
  private readonly ISender _sender;

  public GetPermissionLevelHandler(ModmailDbContext dbContext,
                                   IOptions<BotConfig> options,
                                   ISender sender) {
    _dbContext = dbContext;
    _options = options;
    _sender = sender;
  }

  public async Task<TeamPermissionLevel?> Handle(GetPermissionLevelQuery request, CancellationToken cancellationToken) {
    if (_options.Value.OwnerUsers.Contains(request.UserId)) return TeamPermissionLevel.Owner;

    var roleList = new List<ulong>();
    if (request.IncludeRole) {
      var guild = await _sender.Send(new GetDiscordMainGuildQuery(), cancellationToken);
      var member = await guild.GetMemberAsync(request.UserId);
      roleList.AddRange(member.Roles.Select(x => x.Id));
    }

    var teamMember = await _dbContext.GuildTeamMembers
                                     .Include(x => x.GuildTeam)
                                     .Where(x => (x.Type == TeamMemberDataType.RoleId && roleList.Contains(x.Key))
                                                 || (x.Key == request.UserId && x.Type == TeamMemberDataType.UserId))
                                     .OrderByDescending(x => x.GuildTeam!.PermissionLevel)
                                     .FirstOrDefaultAsync(cancellationToken);
    return teamMember?.GuildTeam?.PermissionLevel;
  }
}