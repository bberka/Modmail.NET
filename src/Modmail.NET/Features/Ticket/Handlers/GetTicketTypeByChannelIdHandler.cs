using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class GetTicketTypeByChannelIdHandler : IRequestHandler<GetTicketTypeByChannelIdQuery, TicketType>
{
  private readonly ModmailDbContext _dbContext;

  public GetTicketTypeByChannelIdHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<TicketType> Handle(GetTicketTypeByChannelIdQuery request, CancellationToken cancellationToken) {
    var result = await _dbContext.Tickets.Where(x => x.ModMessageChannelId == request.ChannelId)
                                 .Select(x => x.TicketType)
                                 .FirstOrDefaultAsync(cancellationToken);
    if (!request.AllowNull && result is null) throw new NotFoundException(LangKeys.TicketType);
    return result;
  }
}