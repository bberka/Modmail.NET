using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.TicketType.Handlers;

public class CheckTicketTypeExistsHandler : IRequestHandler<CheckTicketTypeExistsQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckTicketTypeExistsHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckTicketTypeExistsQuery request, CancellationToken cancellationToken) {
    return await _dbContext.TicketTypes.AnyAsync(x => x.Name == request.NameOrKey || x.Key == request.NameOrKey, cancellationToken);
  }
}