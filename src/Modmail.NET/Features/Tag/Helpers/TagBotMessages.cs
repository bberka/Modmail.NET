using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Tag.Helpers;

public static class TagBotMessages
{
	public static DiscordMessageBuilder TagSent(Database.Entities.Tag message, DiscordUser discordUser, bool ticketChannel) {
		var embed = new DiscordEmbedBuilder()
		            .WithDescription(message.Content)
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TagReceivedColor);

		if (!string.IsNullOrEmpty(message.Title)) embed.WithTitle(message.Title);

		if (ticketChannel) embed.WithUserAsAuthor(discordUser);

		var msg = new DiscordMessageBuilder();
		msg.AddEmbed(embed);
		return msg;
	}
}