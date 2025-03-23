using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;

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
    var closeReason = string.IsNullOrEmpty(request.CloseReason)
                        ? LangProvider.This.GetTranslation(LangKeys.NO_REASON_PROVIDED)
                        : request.CloseReason;
    var closerUser = await _sender.Send(new GetDiscordUserInfoQuery(closerUserId), cancellationToken);
    ArgumentNullException.ThrowIfNull(closerUser);


    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);


    ticket.ClosedDateUtc = DateTime.UtcNow;
    ticket.CloseReason = closeReason;
    ticket.CloserUserId = closerUserId;
    ticket.CloserUser = closerUser;


    _dbContext.Tickets.Update(ticket);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _ = Task.Run(async () => {
      var modChatChannel = request.ModChatChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      await modChatChannel.DeleteAsync(LangProvider.This.GetTranslation(LangKeys.TICKET_CLOSED));
      var pmChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
      if (pmChannel != null) {
        await pmChannel.SendMessageAsync(UserResponses.YourTicketHasBeenClosed(ticket, guildOption));
        if (guildOption.TakeFeedbackAfterClosing && !request.DontSendFeedbackMessage) await pmChannel.SendMessageAsync(UserResponses.GiveFeedbackMessage(ticket, guildOption));
      }

      var logChannel = await _bot.GetLogChannelAsync();
      await logChannel.SendMessageAsync(LogResponses.TicketClosed(ticket));
    }, cancellationToken);
  }
}