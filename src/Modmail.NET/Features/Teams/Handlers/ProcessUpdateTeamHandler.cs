using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class ProcessUpdateTeamHandler : IRequestHandler<ProcessUpdateTeamCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessUpdateTeamHandler(ModmailBot bot,
                                  ModmailDbContext dbContext,
                                  ISender sender) {
    _bot = bot;
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task Handle(ProcessUpdateTeamCommand request, CancellationToken cancellationToken) {


    var anyChanges = request.PermissionLevel.HasValue || request.PingOnNewTicket.HasValue || request.PingOnTicketMessage.HasValue;
    if (!anyChanges) return;

    var team = await _sender.Send(new GetTeamByNameQuery(request.TeamName), cancellationToken);

    var oldPermissionLevel = team.PermissionLevel;
    var oldPingOnNewTicket = team.PingOnNewTicket;
    var oldPingOnNewMessage = team.PingOnNewMessage;
    var oldIsEnabled = team.IsEnabled;
    var oldAllowAccessToWebPanel = team.AllowAccessToWebPanel;
    
    if (request.PermissionLevel.HasValue) team.PermissionLevel = request.PermissionLevel.Value;

    if (request.PingOnNewTicket.HasValue) team.PingOnNewTicket = request.PingOnNewTicket.Value;

    if (request.PingOnTicketMessage.HasValue) team.PingOnNewMessage = request.PingOnTicketMessage.Value;

    if (request.IsEnabled.HasValue) team.IsEnabled = request.IsEnabled.Value;
    
    if (request.AllowAccessToWebPanel.HasValue) team.AllowAccessToWebPanel = request.AllowAccessToWebPanel.Value;


    _dbContext.Update(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}