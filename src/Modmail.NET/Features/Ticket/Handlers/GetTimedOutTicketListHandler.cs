using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTimedOutTicketListHandler : IRequestHandler<GetTimedOutTicketListQuery, List<Entities.Ticket>>
{
  private readonly ModmailDbContext _dbContext;

  public GetTimedOutTicketListHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<Entities.Ticket>> Handle(GetTimedOutTicketListQuery request, CancellationToken cancellationToken) {
    var timeoutHours = request.TimeoutHours;
    if (timeoutHours < Const.TicketTimeoutMinAllowedHours) timeoutHours = Const.DefaultTicketTimeoutHours;
    var timeoutDate = DateTime.UtcNow.AddHours(-timeoutHours);
    var tickets = await _dbContext.Tickets
                                  .Where(x => !x.ClosedDateUtc.HasValue && x.LastMessageDateUtc < timeoutDate)
                                  .ToListAsync(cancellationToken);
    return tickets;
  }
}