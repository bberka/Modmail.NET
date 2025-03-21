using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessAddFeedbackHandler : IRequestHandler<ProcessAddFeedbackCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddFeedbackHandler(ISender sender,
                                   ModmailDbContext dbContext,
                                   ModmailBot bot) {
    _sender = sender;
    _dbContext = dbContext;
    _bot = bot;
  }

  public async Task Handle(ProcessAddFeedbackCommand request, CancellationToken cancellationToken) {
    if (request.StarCount is < 1 or > 5) throw new InvalidOperationException("Star count must be between 1 and 5");
    if (string.IsNullOrEmpty(request.TextInput)) throw new InvalidOperationException("Feedback messageContent is empty");
    if (request.FeedbackMessage is null) throw new InvalidOperationException("Feedback messageContent is null");

    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken) ?? throw new NullReferenceException();
    if (!guildOption.TakeFeedbackAfterClosing) throw new InvalidOperationException("Feedback is not enabled for this guild: " + guildOption.GuildId);

    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeClosed: true), cancellationToken);


    ticket.FeedbackStar = request.StarCount;
    ticket.FeedbackMessage = request.TextInput;
    _dbContext.Update(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      await request.FeedbackMessage.ModifyAsync(x => { x.AddEmbed(UserResponses.FeedbackReceivedUpdateMessage(ticket)); });

      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.FeedbackReceived(ticket));
      }
    }, cancellationToken);
  }
}