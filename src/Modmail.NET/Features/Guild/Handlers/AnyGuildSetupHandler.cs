using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Guild.Handlers;

public class AnyGuildSetupHandler : IRequestHandler<AnyGuildSetupQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public AnyGuildSetupHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(AnyGuildSetupQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildOptions.AnyAsync(cancellationToken);
  }
}