using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Guild.Handlers;

public class ClearGuildOptionHandler : IRequestHandler<ClearGuildOptionCommand>
{
  private readonly ModmailDbContext _dbContext;

  public ClearGuildOptionHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task Handle(ClearGuildOptionCommand request, CancellationToken cancellationToken) {
    var options = await _dbContext.GuildOptions.ToListAsync(cancellationToken);
    _dbContext.GuildOptions.RemoveRange(options);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}