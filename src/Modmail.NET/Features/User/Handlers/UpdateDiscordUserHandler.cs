using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.User.Commands;

namespace Modmail.NET.Features.User.Handlers;

public class UpdateDiscordUserHandler : IRequestHandler<UpdateDiscordUserCommand, UserInformation?>
{
	private readonly ModmailDbContext _dbContext;

	public UpdateDiscordUserHandler(ModmailDbContext dbContext) {
		_dbContext = dbContext;
	}

	public async ValueTask<UserInformation?> Handle(UpdateDiscordUserCommand request, CancellationToken cancellationToken) {
		if (request.DiscordUser is null) return null;
		var entity = UserInformation.FromDiscordUser(request.DiscordUser);

		var dbData = await _dbContext.UserInformation.FindAsync([entity.Id], cancellationToken);
		if (dbData is not null) {
			const int waitHoursAfterUpdate = 24; //updates user information every 24 hours
			if (dbData.UpdateDateUtc.HasValue && dbData.UpdateDateUtc.Value.AddHours(waitHoursAfterUpdate) > UtilDate.GetNow()) return null;
			dbData.Username = entity.Username;
			dbData.AvatarUrl = entity.AvatarUrl;
			dbData.BannerUrl = entity.BannerUrl;
			dbData.Email = entity.Email;
			dbData.Locale = entity.Locale;
			_dbContext.Update(dbData);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return dbData;
		}

		_dbContext.Add(entity);

		return entity;
	}
}