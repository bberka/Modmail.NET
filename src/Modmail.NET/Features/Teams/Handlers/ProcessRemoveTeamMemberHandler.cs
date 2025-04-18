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
    var memberEntity = await _dbContext.GuildTeamMembers
                                       .FirstOrDefaultAsync(x => x.Key == request.TeamMemberKey && x.Type == request.Type, cancellationToken);
    if (memberEntity is null) throw new NotFoundInException(LangKeys.Member, LangKeys.Team);
    _dbContext.GuildTeamMembers.Remove(memberEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}