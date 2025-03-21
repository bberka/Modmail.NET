using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.TicketType.Handlers;

public sealed class GetTicketTypeListHandler : IRequestHandler<GetTicketTypeListQuery, List<Entities.TicketType>>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeListHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<Entities.TicketType>> Handle(GetTicketTypeListQuery request, CancellationToken cancellationToken) {
    var query = _dbContext.TicketTypes.AsQueryable();

    if (request.OnlyActive) query = query.Where(x => x.IsEnabled == true);

    return await query.ToListAsync(cancellationToken);
  }
}