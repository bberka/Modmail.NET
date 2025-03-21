using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Blacklist.Handlers;

public sealed class ProcessRemoveUserFromBlacklistHandler : IRequestHandler<ProcessRemoveUserFromBlacklistCommand, TicketBlacklist>
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
    var authorUserId = request.AuthorUserId == 0
                         ? _bot.Client.CurrentUser.Id
                         : request.AuthorUserId;

    var blacklist = await _sender.Send(new GetBlacklistQuery(request.UserId), cancellationToken);
    _dbContext.Remove(blacklist);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var modUser = await _sender.Send(new GetDiscordUserInfoQuery(authorUserId), cancellationToken);
      var userInfo = await _sender.Send(new GetDiscordUserInfoQuery(request.UserId), cancellationToken);
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var embedLog = LogResponses.BlacklistRemoved(modUser, userInfo);
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(embedLog);
      }

      var member = await _bot.GetMemberFromAnyGuildAsync(request.UserId);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenRemovedFromBlacklist(modUser);
        await member.SendMessageAsync(dmEmbed);
      }
    }, cancellationToken);

    return blacklist;
  }
}