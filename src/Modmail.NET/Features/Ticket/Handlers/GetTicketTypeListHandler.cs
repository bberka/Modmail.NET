using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTicketTypeListHandler : IRequestHandler<GetTicketTypeListQuery, List<TicketType>>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeListHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<TicketType>> Handle(GetTicketTypeListQuery request, CancellationToken cancellationToken) {
    var query = _dbContext.TicketTypes.AsQueryable();

    if (request.OnlyActive) query = query.Where(x => x.IsEnabled == true);

    return await query.ToListAsync(cancellationToken);
  }
}