using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTicketTypeBySearchHandler : IRequestHandler<GetTicketTypeBySearchQuery, TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeBySearchHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<TicketType> Handle(GetTicketTypeBySearchQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Key == request.NameOrKey || x.Name == request.NameOrKey, cancellationToken);
    if (!request.AllowNull && result is null) throw new NotFoundWithException(LangKeys.TicketType, request.NameOrKey);
    return result;
  }
}