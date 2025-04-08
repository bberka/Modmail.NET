using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.User.Queries;

namespace Modmail.NET.Features.User.Handlers;

public class GetDiscordUserInfoDictHandler : IRequestHandler<GetDiscordUserInfoDictQuery, Dictionary<ulong, DiscordUserInfo>>
{
  private readonly ModmailDbContext _dbContext;

  public GetDiscordUserInfoDictHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Dictionary<ulong, DiscordUserInfo>> Handle(GetDiscordUserInfoDictQuery request, CancellationToken cancellationToken) {
    return (await _dbContext.DiscordUserInfos.ToListAsync(cancellationToken)).ToDictionary(x => x.Id, x => x);
  }

  public async Task<List<DiscordUserInfo>> Handle(GetDiscordUserInfoListQuery request, CancellationToken cancellationToken) {
    return await _dbContext.DiscordUserInfos.ToListAsync(cancellationToken);
  }
}