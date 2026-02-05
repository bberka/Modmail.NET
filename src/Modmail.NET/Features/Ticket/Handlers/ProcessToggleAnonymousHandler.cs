using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessToggleAnonymousHandler : IRequestHandler<ProcessToggleAnonymousCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessToggleAnonymousHandler(ISender sender,
                                       ModmailBot bot,
                                       ModmailDbContext dbContext) {
    _sender = sender;
    _bot = bot;
    _dbContext = dbContext;
  }


  public async Task Handle(ProcessToggleAnonymousCommand request, CancellationToken cancellationToken) {
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);


    ticket.Anonymous = !ticket.Anonymous;
    _dbContext.Update(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (ticketChannel is not null) await ticketChannel.SendMessageAsync(TicketBotMessages.Ticket.AnonymousToggled(ticket));
    }, cancellationToken);
  }
}