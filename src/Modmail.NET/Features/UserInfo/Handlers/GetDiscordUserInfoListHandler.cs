using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.UserInfo.Handlers;

public sealed class GetDiscordUserInfoListHandler : IRequestHandler<GetDiscordUserInfoListQuery, List<DiscordUserInfo>>
{
  private readonly ModmailDbContext _dbContext;

  public GetDiscordUserInfoListHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<List<DiscordUserInfo>> Handle(GetDiscordUserInfoListQuery request, CancellationToken cancellationToken) {
    return await _dbContext.DiscordUserInfos.ToListAsync(cancellationToken);
  }
}