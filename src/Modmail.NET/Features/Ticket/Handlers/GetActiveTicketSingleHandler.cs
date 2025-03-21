// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Modmail.NET.Database;
// using Modmail.NET.Exceptions;
//
// namespace Modmail.NET.Features.Ticket.Handlers;
//
// public sealed class GetActiveTicketSingleHandler : IRequestHandler<GetActiveTicketSingleQuery, Entities.Ticket?>
// {
//   private readonly ModmailDbContext _dbContext;
//
//   public GetActiveTicketSingleHandler(ModmailDbContext dbContext) {
//     _dbContext = dbContext;
//   }
//
//   public async Task<Entities.Ticket?> Handle(GetActiveTicketSingleQuery request, CancellationToken cancellationToken) {
//     var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == request.Id && !x.ClosedDateUtc.HasValue, cancellationToken);
//     if (ticket is null) throw new NotFoundException(LangKeys.TICKET);
//     return ticket;
//   }
// }

