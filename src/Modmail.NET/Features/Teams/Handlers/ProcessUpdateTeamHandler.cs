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
    var oldPermissionLevel = request.PermissionLevel;
    var oldPingOnNewTicket = request.PingOnNewTicket;
    var oldPingOnNewMessage = request.PingOnTicketMessage;
    var oldIsEnabled = request.IsEnabled;

    var anyChanges = request.PermissionLevel.HasValue || request.PingOnNewTicket.HasValue || request.PingOnTicketMessage.HasValue;
    if (!anyChanges) return;

    var team = await _sender.Send(new GetTeamByNameQuery(request.TeamName), cancellationToken);

    if (request.PermissionLevel.HasValue) team.PermissionLevel = request.PermissionLevel.Value;

    if (request.PingOnNewTicket.HasValue) team.PingOnNewTicket = request.PingOnNewTicket.Value;

    if (request.PingOnTicketMessage.HasValue) team.PingOnNewMessage = request.PingOnTicketMessage.Value;

    if (request.IsEnabled.HasValue) team.IsEnabled = request.IsEnabled.Value;


    _dbContext.Update(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TeamUpdated(oldPermissionLevel.Value,
                                                                   oldPingOnNewTicket.Value,
                                                                   oldPingOnNewMessage.Value,
                                                                   oldIsEnabled.Value,
                                                                   team.PermissionLevel,
                                                                   team.PingOnNewTicket,
                                                                   team.PingOnNewMessage,
                                                                   team.IsEnabled,
                                                                   team.Name));
      }
    });
  }
}