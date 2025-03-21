// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Modmail.NET.Database;
//
// namespace Modmail.NET.Features.Ticket.Handlers;
//
// public sealed class GetActiveTicketHandler : IRequestHandler<GetActiveTicketQuery, Entities.Ticket?>
// {
//   private readonly ModmailDbContext _dbContext;
//
//   public GetActiveTicketHandler(ModmailDbContext dbContext) {
//     _dbContext = dbContext;
//   }
//
//   public async Task<Entities.Ticket?> Handle(GetActiveTicketQuery request, CancellationToken cancellationToken) {
//     var ticket = await _dbContext.Tickets
//                                  .FirstOrDefaultAsync(x => x.OpenerUserId == request.UserId && !x.ClosedDateUtc.HasValue, cancellationToken);
//     return ticket;
//   }
// }

