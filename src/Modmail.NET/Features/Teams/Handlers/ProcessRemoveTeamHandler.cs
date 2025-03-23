using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessRemoveTeamHandler : IRequestHandler<ProcessRemoveTeamCommand, GuildTeam>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveTeamHandler(ModmailDbContext dbContext,
                                  ModmailBot bot,
                                  ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task<GuildTeam> Handle(ProcessRemoveTeamCommand request, CancellationToken cancellationToken) {
    var team = await _sender.Send(new GetTeamQuery(request.AuthorizedUserId,request.Id), cancellationToken);

    _dbContext.Remove(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
    return team;
  }
}