using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.TicketType.Handlers;

public class GetTicketTypeBySearchHandler : IRequestHandler<GetTicketTypeBySearchQuery, Entities.TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeBySearchHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Entities.TicketType> Handle(GetTicketTypeBySearchQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Key == request.NameOrKey || x.Name == request.NameOrKey, cancellationToken);
    if (!request.AllowNull && result is null) throw new NotFoundWithException(LangKeys.TicketType, request.NameOrKey);
    return result;
  }
}