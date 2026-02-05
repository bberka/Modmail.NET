using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetActiveTicketByUserIdHandler : IRequestHandler<GetTicketByUserIdQuery, Database.Entities.Ticket>
{
  private readonly ModmailDbContext _dbContext;

  public GetActiveTicketByUserIdHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Database.Entities.Ticket> Handle(GetTicketByUserIdQuery request, CancellationToken cancellationToken) {
    var ticket = await _dbContext.Tickets
                                 .FirstOrDefaultAsync(x => x.OpenerUserId == request.UserId && !x.ClosedDateUtc.HasValue, cancellationToken);
    if (!request.AllowNull && ticket is null) throw new NotFoundException(LangKeys.Ticket);
    return ticket;
  }
}