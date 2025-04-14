using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Static;

public static class BlacklistBotMessages
{
  public static DiscordEmbedBuilder YouHaveBeenBlacklisted(string? reason = null) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Lang.YouHaveBeenBlacklisted.Translate())
                .WithDescription(Lang.YouHaveBeenBlacklistedDescription.Translate())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(ModmailColors.ErrorColor);

    if (!string.IsNullOrEmpty(reason)) embed.AddField(Lang.Reason.Translate(), reason);

    return embed;
  }

  public static DiscordEmbedBuilder YouHaveBeenRemovedFromBlacklist(UserInformation user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Lang.YouHaveBeenRemovedFromBlacklist.Translate())
                .WithDescription(Lang.YouHaveBeenRemovedFromBlacklistDescription.Translate())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(ModmailColors.SuccessColor);
    return embed;
  }
}