using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Teams.Handlers;

public sealed class ProcessAddTeamMemberHandler : IRequestHandler<ProcessAddTeamMemberCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddTeamMemberHandler(ModmailDbContext dbContext,
                                     ModmailBot bot,
                                     ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessAddTeamMemberCommand request, CancellationToken cancellationToken) {
    var isUserAlreadyInTeam = await _sender.Send(new CheckUserInAnyTeamQuery(request.MemberId), cancellationToken);
    if (isUserAlreadyInTeam) throw new MemberAlreadyInTeamException();


    var team = await _sender.Send(new GetTeamQuery(request.Id), cancellationToken);

    var memberEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.UserId,
      Key = request.MemberId,
      RegisterDateUtc = DateTime.UtcNow
    };

    _dbContext.GuildTeamMembers.Add(memberEntity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var userInfo = await _sender.Send(new GetDiscordUserInfoQuery(request.MemberId), cancellationToken);
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TeamMemberAdded(userInfo, team.Name));
      }
    }, cancellationToken);
  }
}