using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Permission;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessUserSentMessageHandler : IRequestHandler<ProcessUserSentMessageCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessUserSentMessageHandler(ModmailDbContext dbContext,
                                       ModmailBot bot,
                                       ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessUserSentMessageCommand request, CancellationToken cancellationToken) {
    ArgumentNullException.ThrowIfNull(request.Message);
    await Task.Delay(50, cancellationToken); //wait for privateChannel creation process to finish
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);

    ticket.LastMessageDateUtc = DateTime.UtcNow;

    _dbContext.Update(ticket);
    var ticketMessage = TicketMessage.MapFrom(ticket.Id, request.Message, false);

    _dbContext.Add(ticketMessage);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();


    _ = Task.Run(async () => {
      var mailChannel = await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (mailChannel is not null) {
        var permissions = await _sender.Send(new GetPermissionInfoQuery(), cancellationToken);
        await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(request.Message, permissions));
      }

      var privateChannel = request.PrivateChannel ?? await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
      if (privateChannel is null) throw new NotFoundWithException(LangKeys.Channel, ticket.PrivateMessageChannelId);
      await privateChannel.SendMessageAsync(UserResponses.MessageSent(request.Message));
    }, cancellationToken);
  }
}