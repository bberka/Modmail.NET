using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.TicketType.Handlers;

public class GetTicketTypeHandler : IRequestHandler<GetTicketTypeQuery, Entities.TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Entities.TicketType> Handle(GetTicketTypeQuery request, CancellationToken cancellationToken) {
    var data = await _dbContext.TicketTypes.FindAsync([request.Id], cancellationToken);
    if (!request.AllowNull && data is null) throw new TicketTypeNotExistsException();

    return data;
  }
}