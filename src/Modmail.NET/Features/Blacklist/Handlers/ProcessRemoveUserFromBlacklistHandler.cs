using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Bot;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class ProcessRemoveUserFromBlacklistHandler : IRequestHandler<ProcessRemoveUserFromBlacklistCommand, TicketBlacklist>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveUserFromBlacklistHandler(ModmailBot bot,
                                               ModmailDbContext dbContext,
                                               ISender sender) {
    _bot = bot;
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
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenRemovedFromBlacklist(modUser);
        await member.SendMessageAsync(dmEmbed);
      }
    }, cancellationToken);

    return blacklist;
  }
}