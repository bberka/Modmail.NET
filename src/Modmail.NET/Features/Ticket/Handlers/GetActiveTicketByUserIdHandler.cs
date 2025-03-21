using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class GetActiveTicketByUserIdHandler : IRequestHandler<GetTicketByUserIdQuery, Entities.Ticket>
{
  private readonly ModmailDbContext _dbContext;

  public GetActiveTicketByUserIdHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Entities.Ticket> Handle(GetTicketByUserIdQuery request, CancellationToken cancellationToken) {
    var ticket = await _dbContext.Tickets
                                 .FirstOrDefaultAsync(x => x.OpenerUserId == request.UserId && !x.ClosedDateUtc.HasValue, cancellationToken);
    if (!request.AllowNull && ticket is null) throw new NotFoundException(LangKeys.TICKET);
    return ticket;
  }
}