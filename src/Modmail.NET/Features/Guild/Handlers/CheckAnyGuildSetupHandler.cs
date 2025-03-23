using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Features.Guild.Handlers;

public class CheckAnyGuildSetupHandler : IRequestHandler<CheckAnyGuildSetupQuery, bool>
{
  private readonly ModmailDbContext _dbContext;

  public CheckAnyGuildSetupHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<bool> Handle(CheckAnyGuildSetupQuery request, CancellationToken cancellationToken) {
    return await _dbContext.GuildOptions.AnyAsync(cancellationToken);
  }
}