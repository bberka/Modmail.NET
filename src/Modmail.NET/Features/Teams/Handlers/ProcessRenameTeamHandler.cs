using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class ProcessRenameTeamHandler : IRequestHandler<ProcessRenameTeamCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRenameTeamHandler(ModmailDbContext dbContext,
                                  ModmailBot bot,
                                  ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessRenameTeamCommand request, CancellationToken cancellationToken) {
    var team = await _sender.Send(new GetTeamQuery(request.Id), cancellationToken);

    var oldName = team.Name;
    team.Name = request.NewName;
    _dbContext.Update(team);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      //Don't await this task
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TeamRenamed(oldName, team.Name));
      }
    }, cancellationToken);
  }
}