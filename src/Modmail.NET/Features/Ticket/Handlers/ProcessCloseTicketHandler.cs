using DSharpPlus.Exceptions;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Features.Bot;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessCloseTicketHandler : IRequestHandler<ProcessCloseTicketCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessCloseTicketHandler(ModmailDbContext dbContext, ModmailBot bot, ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessCloseTicketCommand request, CancellationToken cancellationToken) {
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId,
                                                       MustBeOpen: true), cancellationToken);

    var closerUserId = request.CloserUserId == 0
                         ? _bot.Client.CurrentUser.Id
                         : request.CloserUserId;
    var closeReason = request.CloseReason?.Trim();
    if (string.IsNullOrEmpty(closeReason)) closeReason = null;

    var closerUser = await _sender.Send(new GetDiscordUserInfoQuery(closerUserId), cancellationToken);
    ArgumentNullException.ThrowIfNull(closerUser);


    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);


    ticket.ClosedDateUtc = UtilDate.GetNow();
    ticket.CloseReason = closeReason;
    ticket.CloserUserId = closerUserId;
    ticket.CloserUser = closerUser;


    _dbContext.Tickets.Update(ticket);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _ = Task.Run(async () => {
      var modChatChannel = request.ModChatChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      await modChatChannel.DeleteAsync(LangProvider.This.GetTranslation(LangKeys.TicketClosed));
      try {
        var pmChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
        await pmChannel.SendMessageAsync(UserResponses.YourTicketHasBeenClosed(ticket, guildOption));
        if (guildOption.TakeFeedbackAfterClosing && !request.DontSendFeedbackMessage) await pmChannel.SendMessageAsync(UserResponses.GiveFeedbackMessage(ticket, guildOption));
      }
      catch (NotFoundException) {
        //ignored
      }

      var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
      await logChannel.SendMessageAsync(LogResponses.TicketClosed(ticket));
    }, cancellationToken);
  }
}