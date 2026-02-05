using DSharpPlus.Entities;
using Modmail.NET.Entities;

namespace Modmail.NET.Extensions;

public static class ExtEmbed
{
    public static DiscordEmbedBuilder AddAttachment(this DiscordEmbedBuilder builder, IReadOnlyList<DiscordAttachment>? discordAttachments)
    {
        if (discordAttachments == null || discordAttachments.Count == 0) return builder;
        for (var i = 0; i < discordAttachments.Count; i++)
            builder.AddField($"{LangData.This.GetTranslation(LangKeys.ATTACHMENT)} {i + 1}", discordAttachments[i].Url);
        return builder;
    }

    public static DiscordEmbedBuilder AddAttachment(this DiscordEmbedBuilder builder, DiscordAttachment discordAttachment)
    {
        builder.AddField($"{LangData.This.GetTranslation(LangKeys.ATTACHMENT)}", discordAttachment.Url);
        return builder;
    }

    public static DiscordEmbedBuilder WithCustomTimestamp(this DiscordEmbedBuilder builder)
    {
        return builder.WithTimestamp(UtilDate.GetNow());
    }

    public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder)
    {
        var guildInfo = ModmailBot.This.GetMainGuildAsync()
            .GetAwaiter()
            .GetResult();
        return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
    }

    public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder, GuildOption guildInfo)
    {
        return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
    }

    public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder, DiscordGuild guildInfo)
    {
        return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
    }

    public static DiscordEmbedBuilder WithUserAsAuthor(this DiscordEmbedBuilder builder, DiscordUser user)
    {
        return builder.WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl);
    }

    public static DiscordEmbedBuilder WithUserAsAuthor(this DiscordEmbedBuilder builder, DiscordUserInfo user)
    {
        return builder.WithAuthor(user.Username, iconUrl: user.AvatarUrl);
    }
}