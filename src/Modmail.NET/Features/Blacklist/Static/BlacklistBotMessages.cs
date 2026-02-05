using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Static;

public static class BlacklistBotMessages
{
  public static DiscordEmbedBuilder YouHaveBeenBlacklisted(string reason = null) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YouHaveBeenBlacklisted.GetTranslation())
                .WithDescription(LangKeys.YouHaveBeenBlacklistedDescription.GetTranslation())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(ModmailColors.ErrorColor);

    if (!string.IsNullOrEmpty(reason)) embed.AddField(LangKeys.Reason.GetTranslation(), reason);

    return embed;
  }

  public static DiscordEmbedBuilder YouHaveBeenRemovedFromBlacklist(DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YouHaveBeenRemovedFromBlacklist.GetTranslation())
                .WithDescription(LangKeys.YouHaveBeenRemovedFromBlacklistDescription.GetTranslation())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithUserAsAuthor(user)
                .WithColor(ModmailColors.SuccessColor);
    return embed;
  }
}