using DSharpPlus.Entities;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Common.Extensions;

public static class EmbedExtensions
{
	public static DiscordMessageBuilder AddAttachments(this DiscordMessageBuilder builder, TicketMessageAttachment[] attachments) {
		if (attachments.Length == 0) return builder;

		foreach (var attachment in attachments) {
			var path = UtilAttachment.GetLocalPath(attachment);
			builder.AddFile(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), AddFileOptions.None);
		}

		return builder;
	}

	public static DiscordEmbedBuilder WithCustomTimestamp(this DiscordEmbedBuilder builder) {
		return builder.WithTimestamp(UtilDate.GetNow());
	}

	public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder, Option info) {
		return builder.WithFooter(info.Name, info.IconUrl);
	}

	public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder, DiscordGuild guildInfo) {
		return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
	}

	public static DiscordEmbedBuilder WithUserAsAuthor(this DiscordEmbedBuilder builder, DiscordUser user) {
		return builder.WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl);
	}

	public static DiscordEmbedBuilder WithUserAsAuthor(this DiscordEmbedBuilder builder, UserInformation user) {
		return builder.WithAuthor(user.Username, iconUrl: user.AvatarUrl);
	}
}