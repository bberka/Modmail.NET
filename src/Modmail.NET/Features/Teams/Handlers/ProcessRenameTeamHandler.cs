using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Language;

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
    var team = await _dbContext.Teams.FindAsync([request.Id], cancellationToken);
    if (team is null) throw new ModmailBotException(Lang.TeamNotFound);

    team.Name = request.NewName;
    _dbContext.Update(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}