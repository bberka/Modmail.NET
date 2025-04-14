using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessRemoveTicketTypeHandler : IRequestHandler<ProcessRemoveTicketTypeCommand, TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public ProcessRemoveTicketTypeHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<TicketType> Handle(ProcessRemoveTicketTypeCommand request, CancellationToken cancellationToken) {
    var ticketType = await _dbContext.TicketTypes.FindAsync([request.Id], cancellationToken)
                     ?? throw new ModmailBotException(Lang.TicketTypeNotFound);

    var allTicketsByType = await _dbContext.Tickets
                                           .FilterByTypeId(request.Id)
                                           .ToListAsync(cancellationToken);

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