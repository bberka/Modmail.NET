using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class ProcessAddRoleToTeamHandler : IRequestHandler<ProcessAddRoleToTeamCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddRoleToTeamHandler(ModmailDbContext dbContext,
                                     ModmailBot bot,
                                     ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessAddRoleToTeamCommand request, CancellationToken cancellationToken) {
    var isRoleAlreadyInTeam = await _sender.Send(new CheckRoleInAnyTeamQuery(request.Role.Id), cancellationToken);
    if (isRoleAlreadyInTeam) throw new RoleAlreadyInTeamException();

    var roleEntity = new GuildTeamMember {
      GuildTeamId = request.Id,
      Type = TeamMemberDataType.RoleId,
      Key = request.Role.Id,
      RegisterDateUtc = DateTime.UtcNow
    };

    _dbContext.GuildTeamMembers.Add(roleEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();


    _ = Task.Run(async () => {
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        var teamName = await _dbContext.GuildTeams.Where(x => x.Id == request.Id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken);
        await logChannel.SendMessageAsync(LogResponses.TeamRoleAdded(request.Role, teamName));
      }
    }, cancellationToken);
  }
}