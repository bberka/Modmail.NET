using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Features.Teams.Static;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessAddTeamMemberHandler : IRequestHandler<ProcessAddTeamMemberCommand>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddTeamMemberHandler(ModmailDbContext dbContext,
                                     ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task Handle(ProcessAddTeamMemberCommand request, CancellationToken cancellationToken) {
    var isUserAlreadyInTeam = await _sender.Send(new CheckUserInAnyTeamQuery(request.MemberId), cancellationToken);
    if (isUserAlreadyInTeam) throw new MemberAlreadyInTeamException();


    var team = await _sender.Send(new GetTeamQuery(request.AuthorizedUserId, request.Id), cancellationToken);

    var memberEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.UserId,
      Key = request.MemberId,
      RegisterDateUtc = UtilDate.GetNow()
    };

    _dbContext.GuildTeamMembers.Add(memberEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}