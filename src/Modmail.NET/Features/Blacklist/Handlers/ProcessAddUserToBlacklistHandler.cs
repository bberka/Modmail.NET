using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Blacklist.Handlers;

public sealed class ProcessAddUserToBlacklistHandler : IRequestHandler<ProcessAddUserToBlacklistCommand, TicketBlacklist>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddUserToBlacklistHandler(ModmailDbContext dbContext,
                                          ModmailBot bot,
                                          ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task<TicketBlacklist> Handle(ProcessAddUserToBlacklistCommand request, CancellationToken cancellationToken) {
    var modId = request.ModId == 0
                  ? _bot.Client.CurrentUser.Id
                  : request.ModId;
    var activeTicket = await _sender.Send(new GetTicketByUserIdQuery(request.UserId, true, true), cancellationToken);
    if (activeTicket is not null)
      await _sender.Send(new ProcessCloseTicketCommand(activeTicket.Id,
                                                       request.UserId,
                                                       LangProvider.This.GetTranslation(LangKeys.TICKET_CLOSED_DUE_TO_BLACKLIST),
                                                       DontSendFeedbackMessage: true),
                         cancellationToken);

    var activeBlock = await _sender.Send(new CheckUserBlacklistStatusQuery(request.UserId), cancellationToken);
    if (activeBlock) throw new UserAlreadyBlacklistedException();

    var reason = string.IsNullOrEmpty(request.Reason)
                   ? LangProvider.This.GetTranslation(LangKeys.NO_REASON_PROVIDED)
                   : request.Reason;

    var blackList = new TicketBlacklist {
      Id = Guid.NewGuid(),
      Reason = reason,
      DiscordUserId = request.UserId,
      RegisterDateUtc = DateTime.UtcNow
    };

    _dbContext.Add(blackList);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();


    _ = Task.Run(async () => {
      var user = await _sender.Send(new GetDiscordUserInfoQuery(request.UserId), cancellationToken);
      var modUser = await _sender.Send(new GetDiscordUserInfoQuery(modId), cancellationToken);

      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var embedLog = LogResponses.BlacklistAdded(modUser, user, reason);
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(embedLog);
      }

      var member = await _bot.GetMemberFromAnyGuildAsync(user.Id);
      if (member is not null) {
        var dmEmbed = UserResponses.YouHaveBeenBlacklisted(reason);
        await member.SendMessageAsync(dmEmbed);
      }
    }, cancellationToken);

    return blackList;
  }
}