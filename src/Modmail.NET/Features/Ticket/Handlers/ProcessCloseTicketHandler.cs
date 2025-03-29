using DSharpPlus.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;
using Modmail.NET.Features.Bot;
using Modmail.NET.Features.Guild;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessCloseTicketHandler : IRequestHandler<ProcessCloseTicketCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly IOptions<BotConfig> _options;
  private readonly ISender _sender;

  public ProcessCloseTicketHandler(ModmailDbContext dbContext,
                                   ModmailBot bot,
                                   ISender sender,
                                   IOptions<BotConfig> options) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
    _options = options;
  }

  public async Task Handle(ProcessCloseTicketCommand request, CancellationToken cancellationToken) {
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId,
                                                       MustBeOpen: true), cancellationToken);

    var closerUserId = request.CloserUserId == 0
                         ? _bot.Client.CurrentUser.Id
                         : request.CloserUserId;
    var closeReason = request.CloseReason?.Trim();
    if (string.IsNullOrEmpty(closeReason)) closeReason = null;


    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);


    ticket.ClosedDateUtc = UtilDate.GetNow();
    ticket.CloseReason = closeReason;
    ticket.CloserUserId = closerUserId;


    _dbContext.Tickets.Update(ticket);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _ = Task.Run(async () => {
      var sendLinkToUser = Uri.TryCreate(_options.Value.Domain, UriKind.Absolute, out var uri) && guildOption.SendTranscriptLinkToUser;
      Uri transcriptUri = null;
      if (sendLinkToUser)
        try {
          transcriptUri = new Uri(uri, "transcript/" + request.TicketId);
        }
        catch (UriFormatException) {
          transcriptUri = null;
        }


      var modChatChannel = request.ModChatChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      await modChatChannel.DeleteAsync(LangProvider.This.GetTranslation(LangKeys.TicketClosed));
      try {
        var pmChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
        await pmChannel.SendMessageAsync(UserResponses.YourTicketHasBeenClosed(ticket, guildOption, transcriptUri));
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