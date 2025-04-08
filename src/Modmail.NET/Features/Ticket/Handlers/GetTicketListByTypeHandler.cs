using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTicketListByTypeHandler : IRequestHandler<GetTicketListByTypeQuery, List<Database.Entities.Ticket>>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketListByTypeHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<Database.Entities.Ticket>> Handle(GetTicketListByTypeQuery request, CancellationToken cancellationToken) {
    var tickets = await _dbContext.Tickets
                                  .Where(x => x.TicketTypeId == request.TicketTypeId)
                                  .ToListAsync(cancellationToken);
    return tickets;
  }
}