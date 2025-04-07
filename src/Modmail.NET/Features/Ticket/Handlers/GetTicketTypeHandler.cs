using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTicketTypeHandler : IRequestHandler<GetTicketTypeQuery, TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<TicketType> Handle(GetTicketTypeQuery request, CancellationToken cancellationToken) {
    var data = await _dbContext.TicketTypes.FindAsync([request.Id], cancellationToken);
    if (!request.AllowNull && data is null) throw new TicketTypeNotExistsException();

    return data;
  }
}