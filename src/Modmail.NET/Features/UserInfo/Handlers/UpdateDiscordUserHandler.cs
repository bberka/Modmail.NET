using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.UserInfo.Handlers;

public class UpdateDiscordUserHandler : IRequestHandler<UpdateDiscordUserCommand, DiscordUserInfo>
{
  private readonly ModmailDbContext _dbContext;

  public UpdateDiscordUserHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<DiscordUserInfo> Handle(UpdateDiscordUserCommand request, CancellationToken cancellationToken) {
    //TODO: handle null returns better
    if (request.DiscordUser is null) return null;
    var entity = DiscordUserInfo.FromDiscordUser(request.DiscordUser);

    var dbData = await _dbContext.DiscordUserInfos.FindAsync([entity.Id], cancellationToken);
    if (dbData is not null) {
      const int waitHoursAfterUpdate = 24; //updates user information every 24 hours
      if (dbData.UpdateDateUtc.HasValue && dbData.UpdateDateUtc.Value.AddHours(waitHoursAfterUpdate) > UtilDate.GetNow()) return null;
      dbData.Username = entity.Username;
      dbData.AvatarUrl = entity.AvatarUrl;
      dbData.BannerUrl = entity.BannerUrl;
      dbData.Email = entity.Email;
      dbData.Locale = entity.Locale;
      _dbContext.DiscordUserInfos.Update(dbData);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return dbData;
    }

    entity.RegisterDateUtc = UtilDate.GetNow();
    _dbContext.DiscordUserInfos.Add(entity);

    return entity;
  }
}