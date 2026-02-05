using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class CheckActiveTicketHandler : IRequestHandler<CheckActiveTicketQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckActiveTicketHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckActiveTicketQuery request, CancellationToken cancellationToken) {
    return await _dbContext.Tickets.Where(x => x.Id == request.TicketId && !x.ClosedDateUtc.HasValue).AnyAsync(cancellationToken);
  }
}