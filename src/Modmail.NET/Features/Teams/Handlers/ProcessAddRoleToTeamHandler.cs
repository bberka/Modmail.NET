using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessAddRoleToTeamHandler : IRequestHandler<ProcessAddRoleToTeamCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddRoleToTeamHandler(ModmailDbContext dbContext,
                                     ModmailBot bot,
                                     ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessAddRoleToTeamCommand request, CancellationToken cancellationToken) {
    var isRoleAlreadyInTeam = await _sender.Send(new CheckRoleInAnyTeamQuery(request.Role.Id), cancellationToken);
    if (isRoleAlreadyInTeam) throw new RoleAlreadyInTeamException();

    var roleEntity = new GuildTeamMember {
      GuildTeamId = request.Id,
      Type = TeamMemberDataType.RoleId,
      Key = request.Role.Id,
      RegisterDateUtc = DateTime.UtcNow
    };

    _dbContext.GuildTeamMembers.Add(roleEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}