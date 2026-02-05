using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.Teams.Queries;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessRemoveTeamHandler : IRequestHandler<ProcessRemoveTeamCommand, GuildTeam>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveTeamHandler(ModmailDbContext dbContext,
                                  ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<GuildTeam> Handle(ProcessRemoveTeamCommand request, CancellationToken cancellationToken) {
    var team = await _sender.Send(new GetTeamQuery(request.AuthorizedUserId, request.Id), cancellationToken);

    _dbContext.Remove(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
    return team;
  }
}