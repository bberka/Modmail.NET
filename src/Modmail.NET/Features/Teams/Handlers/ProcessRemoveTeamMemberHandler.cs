using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessRemoveTeamMemberHandler : IRequestHandler<ProcessRemoveTeamMemberCommand>
{
  private readonly ModmailDbContext _dbContext;

  public ProcessRemoveTeamMemberHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task Handle(ProcessRemoveTeamMemberCommand request, CancellationToken cancellationToken) {
    var memberEntity = await _dbContext.TeamUsers
                                       .Include(x => x.Team)
                                       .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
    if (memberEntity is null) throw new ModmailBotException(Lang.MemberNotFoundInTeam);
    if (memberEntity.Team!.SuperUserTeam) throw new ModmailBotException(Lang.CanNotRemoveTeamMemberDueToConfigTeam);

    _dbContext.Remove(memberEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}