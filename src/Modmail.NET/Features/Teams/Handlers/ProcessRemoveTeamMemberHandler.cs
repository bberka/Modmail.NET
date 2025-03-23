using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class ProcessRemoveTeamMemberHandler : IRequestHandler<ProcessRemoveTeamMemberCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveTeamMemberHandler(ModmailDbContext dbContext,
                                        ModmailBot bot,
                                        ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessRemoveTeamMemberCommand request, CancellationToken cancellationToken) {
    var memberEntity = await _dbContext.GuildTeamMembers
                                       .FirstOrDefaultAsync(x => x.Key == request.TeamMemberKey && x.Type == request.Type, cancellationToken);
    if (memberEntity is null) throw new NotFoundInException(LangKeys.MEMBER, LangKeys.TEAM);
    _dbContext.GuildTeamMembers.Remove(memberEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}