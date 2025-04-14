using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Blacklist.Commands;
using Modmail.NET.Features.Blacklist.Static;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class ProcessAddUserToBlacklistHandler : IRequestHandler<ProcessAddUserToBlacklistCommand, Database.Entities.Blacklist>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddUserToBlacklistHandler(ModmailDbContext dbContext,
                                          ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<Database.Entities.Blacklist> Handle(ProcessAddUserToBlacklistCommand request, CancellationToken cancellationToken) {
    var activeTicket = await _dbContext.Tickets.FilterActive().FirstOrDefaultAsync(cancellationToken);
    if (activeTicket is not null)
      await _sender.Send(new ProcessCloseTicketCommand(activeTicket.Id,
                                                       request.UserId,
                                                       LangProvider.This.GetTranslation(Lang.TicketClosedDueToBlacklist),
                                                       DontSendFeedbackMessage: true),
                         cancellationToken);

    var activeBlock = await _dbContext.Blacklists.FilterByUserId(request.UserId).AnyAsync(cancellationToken);
    if (activeBlock) throw new ModmailBotException(Lang.UserAlreadyBlacklisted);

    var reason = string.IsNullOrEmpty(request.Reason)
                   ? LangProvider.This.GetTranslation(Lang.NoReasonProvided)
                   : request.Reason;

    var blackList = new Database.Entities.Blacklist {
      Reason = reason,
      UserId = request.UserId
    };

    _dbContext.Add(blackList);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();


    _ = Task.Run(async () => {
      var user = await _sender.Send(new GetDiscordUserInfoQuery(request.UserId), cancellationToken);
      var modUser = await _sender.Send(new GetDiscordUserInfoQuery(request.AuthorizedUserId), cancellationToken);

      var embedLog = LogBotMessages.BlacklistAdded(modUser, user, reason);
      var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
      await logChannel.SendMessageAsync(embedLog);

      var member = await _sender.Send(new GetDiscordMemberQuery(user.Id), cancellationToken);
      if (member is not null) {
        var dmEmbed = BlacklistBotMessages.YouHaveBeenBlacklisted(reason);
        await member.SendMessageAsync(dmEmbed);
      }
    }, cancellationToken);

    return blackList;
  }
}