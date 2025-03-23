using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTicketHandler : IRequestHandler<GetTicketQuery, Entities.Ticket>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Entities.Ticket> Handle(GetTicketQuery request, CancellationToken cancellationToken) {
    var ticket = await _dbContext.Tickets.FindAsync([request.Id], cancellationToken);
    if (ticket is null) {
      if (!request.AllowNull) throw new NotFoundException(LangKeys.TICKET);
      return null;
    }

    if (ticket.OpenerUser is null) throw new InvalidOperationException("OpenerUserInfo is null");

    if (!ticket.ClosedDateUtc.HasValue && request.MustBeClosed) throw new TicketMustBeClosedException();
    if (ticket.ClosedDateUtc.HasValue && request.MustBeOpen) throw new TicketAlreadyClosedException();
    return ticket;
  }
}