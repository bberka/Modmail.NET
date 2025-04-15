using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.User.Queries;
using Serilog;

namespace Modmail.NET.Features.User.Jobs;

public class DiscordUserInfoSyncJob : HangfireRecurringJobBase
{
	private readonly IServiceScopeFactory _scopeFactory;

	public DiscordUserInfoSyncJob(IServiceScopeFactory scopeFactory) : base("discord-user-info-sync-job", Cron.Hourly()) {
		_scopeFactory = scopeFactory;
	}


	public override async Task Execute() {
		var scope = _scopeFactory.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
		var sender = scope.ServiceProvider.GetRequiredService<ISender>();
		var guild = await sender.Send(new GetDiscordMainServerQuery());
		var members = await guild.GetAllMembersAsync().ToListAsync();
		// var allDbUsers = await DiscordUserInfo.GetAllAsync();
		var allDbUsers = await sender.Send(new GetDiscordUserInfoListQuery());
		var convertedDiscordUsers = members.Select(UserInformation.FromDiscordMember).ToList();

		var updateList = new List<UserInformation>();
		foreach (var dbUser in allDbUsers) {
			var discordUser = convertedDiscordUsers.FirstOrDefault(x => x.Id == dbUser.Id);
			if (discordUser is null) continue;
			var isAllSame = dbUser.AvatarUrl == discordUser.AvatarUrl &&
			                dbUser.Username == discordUser.Username &&
			                dbUser.BannerUrl == discordUser.BannerUrl &&
			                dbUser.Email == discordUser.Email &&
			                dbUser.Locale == discordUser.Locale;
			if (isAllSame) continue;
			dbUser.AvatarUrl = discordUser.AvatarUrl;
			dbUser.Username = discordUser.Username;
			dbUser.BannerUrl = discordUser.BannerUrl;
			dbUser.Email = discordUser.Email;
			dbUser.Locale = discordUser.Locale;
			updateList.Add(dbUser);
		}

		var addedUsers = convertedDiscordUsers.Where(x => allDbUsers.All(y => y.Id != x.Id)).ToList();
		dbContext.AddRange(addedUsers);
		dbContext.UpdateRange(updateList);
		var affected = await dbContext.SaveChangesAsync();
		Log.Information("DiscordUserInfo table synced with discord guild {Affected}", affected);
	}
}