using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Permission;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessCreateTeamHandler : IRequestHandler<ProcessCreateTeamCommand, GuildTeam>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessCreateTeamHandler(ModmailDbContext dbContext,
                                  ModmailBot bot,
                                  ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task<GuildTeam> Handle(ProcessCreateTeamCommand request, CancellationToken cancellationToken) {
    var exists = await _sender.Send(new CheckTeamExistsQuery(request.AuthorizedUserId,request.TeamName), cancellationToken);
    if (exists) throw new TeamAlreadyExistsException();
    
    var userPermissionLevel = await _sender.Send(new GetPermissionLevelQuery(request.AuthorizedUserId, true), cancellationToken) ?? throw new NullReferenceException(nameof(TeamPermissionLevel));
    if (request.PermissionLevel > userPermissionLevel) {
      throw new InvalidOperationException("Can not set higher team permission than self user");
    }

    var team = new GuildTeam {
      Name = request.TeamName,
      RegisterDateUtc = DateTime.UtcNow,
      IsEnabled = true,
      GuildTeamMembers = [],
      UpdateDateUtc = null,
      Id = Guid.NewGuid(),
      PermissionLevel = request.PermissionLevel,
      PingOnNewMessage = request.PingOnTicketMessage,
      PingOnNewTicket = request.PingOnNewTicket,
      AllowAccessToWebPanel = request.AllowAccessToWebPanel
    };

    _dbContext.Add(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
    return team;
  }
}