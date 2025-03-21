using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class GetTicketListByTypeHandler : IRequestHandler<GetTicketListByTypeQuery, List<Entities.Ticket>>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketListByTypeHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<Entities.Ticket>> Handle(GetTicketListByTypeQuery request, CancellationToken cancellationToken) {
    var tickets = await _dbContext.Tickets
                                  .Where(x => x.TicketTypeId == request.TicketTypeId)
                                  .ToListAsync(cancellationToken);
    return tickets;
  }
}