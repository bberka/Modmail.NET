using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.Teams.Static;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessAddRoleToTeamHandler : IRequestHandler<ProcessAddRoleToTeamCommand>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddRoleToTeamHandler(ModmailDbContext dbContext,
                                     ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task Handle(ProcessAddRoleToTeamCommand request, CancellationToken cancellationToken) {
    var isRoleAlreadyInTeam = await _sender.Send(new CheckRoleInAnyTeamQuery(request.Role.Id), cancellationToken);
    if (isRoleAlreadyInTeam) throw new RoleAlreadyInTeamException();

    var roleEntity = new GuildTeamMember {
      GuildTeamId = request.Id,
      Type = TeamMemberDataType.RoleId,
      Key = request.Role.Id,
      RegisterDateUtc = UtilDate.GetNow()
    };

    _dbContext.GuildTeamMembers.Add(roleEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}