using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Bot;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class ProcessRemoveUserFromBlacklistHandler : IRequestHandler<ProcessRemoveUserFromBlacklistCommand, TicketBlacklist>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveUserFromBlacklistHandler(ModmailDbContext dbContext,
                                               ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<TicketBlacklist> Handle(ProcessRemoveUserFromBlacklistCommand request, CancellationToken cancellationToken) {
    var blacklist = await _sender.Send(new GetBlacklistQuery(request.AuthorizedUserId, request.UserId), cancellationToken);
    _dbContext.Remove(blacklist);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var modUser = await _sender.Send(new GetDiscordUserInfoQuery(request.AuthorizedUserId), cancellationToken);
      var member = await _sender.Send(new GetDiscordMemberQuery(request.UserId), cancellationToken);
      var memberInfo = DiscordUserInfo.FromDiscordMember(member);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenRemovedFromBlacklist(modUser);
        await member.SendMessageAsync(dmEmbed);
      }

      var embedLog = LogResponses.BlacklistRemoved(modUser, memberInfo);
      var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
      await logChannel.SendMessageAsync(embedLog);
    }, cancellationToken);

    return blacklist;
  }
}