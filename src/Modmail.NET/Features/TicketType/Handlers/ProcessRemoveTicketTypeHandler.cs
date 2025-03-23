using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Ticket;

namespace Modmail.NET.Features.TicketType.Handlers;

public class ProcessRemoveTicketTypeHandler : IRequestHandler<ProcessRemoveTicketTypeCommand, Entities.TicketType>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveTicketTypeHandler(ModmailDbContext dbContext,
                                        ModmailBot bot,
                                        ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task<Entities.TicketType> Handle(ProcessRemoveTicketTypeCommand request, CancellationToken cancellationToken) {
    var ticketType = await _sender.Send(new GetTicketTypeQuery(request.Id), cancellationToken);
    var allTicketsByType = await _sender.Send(new GetTicketListByTypeQuery(ticketType.Id), cancellationToken);

    if (allTicketsByType.Count > 0) {
      foreach (var ticket in allTicketsByType) {
        ticket.TicketTypeId = null;
        ticket.TicketType = null;
      }

      _dbContext.UpdateRange(allTicketsByType);
    }

    _dbContext.Remove(ticketType);

    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    return ticketType;
  }
}