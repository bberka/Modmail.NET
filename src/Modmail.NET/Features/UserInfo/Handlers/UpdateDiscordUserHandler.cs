using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;

namespace Modmail.NET.Features.UserInfo.Handlers;

public class UpdateDiscordUserHandler : IRequestHandler<UpdateDiscordUserCommand, DiscordUserInfo>
{
  private readonly ModmailDbContext _dbContext;

  public UpdateDiscordUserHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<DiscordUserInfo> Handle(UpdateDiscordUserCommand request, CancellationToken cancellationToken) {
    if (request.DiscordUser is null) return default; //TODO: Check this and handle it better
    var entity = DiscordUserInfo.FromDiscordUser(request.DiscordUser);

    var dbData = await _dbContext.DiscordUserInfos.FindAsync([entity.Id], cancellationToken);
    if (dbData is not null) {
      const int waitHoursAfterUpdate = 24; //updates user information every 24 hours
      var lastUpdate = dbData.UpdateDateUtc ?? dbData.RegisterDateUtc;
      if (lastUpdate.AddHours(waitHoursAfterUpdate) > DateTime.Now) return default; //TODO: Check this handle it better
      dbData.UpdateDateUtc = DateTime.UtcNow;
      dbData.Username = entity.Username;
      dbData.AvatarUrl = entity.AvatarUrl;
      dbData.BannerUrl = entity.BannerUrl;
      dbData.Email = entity.Email;
      dbData.Locale = entity.Locale;
      _dbContext.DiscordUserInfos.Update(dbData);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return dbData;
    }

    entity.RegisterDateUtc = DateTime.UtcNow;
    _dbContext.DiscordUserInfos.Add(entity);

    return entity;
  }
}