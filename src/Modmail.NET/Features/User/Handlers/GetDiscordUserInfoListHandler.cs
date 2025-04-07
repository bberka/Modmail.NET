using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.User.Queries;

namespace Modmail.NET.Features.User.Handlers;

public class GetDiscordUserInfoListHandler : IRequestHandler<GetDiscordUserInfoListQuery, List<DiscordUserInfo>>
{
  private readonly ModmailDbContext _dbContext;

  public GetDiscordUserInfoListHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<DiscordUserInfo>> Handle(GetDiscordUserInfoListQuery request, CancellationToken cancellationToken) {
    return await _dbContext.DiscordUserInfos.ToListAsync(cancellationToken);
  }
}