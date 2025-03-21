using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Guild.Handlers;

public sealed class GetGuildOptionHandler : IRequestHandler<GetGuildOptionQuery, GuildOption>
{
  private readonly ModmailDbContext _dbContext;
  private readonly IOptions<BotConfig> _options;

  public GetGuildOptionHandler(ModmailDbContext dbContext,
                               IOptions<BotConfig> options) {
    _dbContext = dbContext;
    _options = options;
  }

  public async Task<GuildOption> Handle(GetGuildOptionQuery request, CancellationToken cancellationToken) {
    var option = await _dbContext.GuildOptions.FirstOrDefaultAsync(x => x.GuildId == _options.Value.MainServerId, cancellationToken);
    if (!request.AllowNull && option is null) throw new ServerIsNotSetupException();
    return option;
  }
}