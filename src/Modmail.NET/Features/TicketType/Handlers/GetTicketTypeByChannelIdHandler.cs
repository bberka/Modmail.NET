using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.TicketType.Handlers;

public class GetTicketTypeByChannelIdHandler : IRequestHandler<GetTicketTypeByChannelIdQuery, Entities.TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeByChannelIdHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Entities.TicketType> Handle(GetTicketTypeByChannelIdQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.Tickets.Where(x => x.ModMessageChannelId == request.ChannelId)
                                 .Select(x => x.TicketType)
                                 .FirstOrDefaultAsync(cancellationToken);
    if (!request.AllowNull && result is null) throw new NotFoundException(LangKeys.TicketType);
    return result;
  }
}