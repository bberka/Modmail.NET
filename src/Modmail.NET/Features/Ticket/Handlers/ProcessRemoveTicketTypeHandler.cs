using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessRemoveTicketTypeHandler : IRequestHandler<ProcessRemoveTicketTypeCommand, TicketType>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessRemoveTicketTypeHandler(ModmailDbContext dbContext,
                                        ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<TicketType> Handle(ProcessRemoveTicketTypeCommand request, CancellationToken cancellationToken) {
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