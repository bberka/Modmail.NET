using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessAddTeamMemberHandler : IRequestHandler<ProcessAddTeamMemberCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddTeamMemberHandler(ModmailDbContext dbContext,
                                     ModmailBot bot,
                                     ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessAddTeamMemberCommand request, CancellationToken cancellationToken) {
    var isUserAlreadyInTeam = await _sender.Send(new CheckUserInAnyTeamQuery(request.MemberId), cancellationToken);
    if (isUserAlreadyInTeam) throw new MemberAlreadyInTeamException();


    var team = await _sender.Send(new GetTeamQuery(request.Id), cancellationToken);

    var memberEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.UserId,
      Key = request.MemberId,
      RegisterDateUtc = DateTime.UtcNow
    };

    _dbContext.GuildTeamMembers.Add(memberEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}