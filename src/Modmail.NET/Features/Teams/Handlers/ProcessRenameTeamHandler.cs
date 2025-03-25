using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessRenameTeamHandler : IRequestHandler<ProcessRenameTeamCommand>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRenameTeamHandler(ModmailDbContext dbContext,
                                  ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task Handle(ProcessRenameTeamCommand request, CancellationToken cancellationToken) {
    var team = await _sender.Send(new GetTeamQuery(request.AuthorizedUserId,request.Id), cancellationToken);
    team.Name = request.NewName;
    _dbContext.Update(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}