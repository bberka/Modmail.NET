using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Features.Bot;
using Modmail.NET.Utils;

namespace Modmail.NET.Extensions;

public static class ExtEmbed
{
  public static DiscordEmbedBuilder AddAttachment(this DiscordEmbedBuilder builder, TicketMessageAttachment[] attachments) {
    if (attachments == null || attachments.Length == 0) return builder;
    foreach (var t in attachments)
      builder.AddField($"{LangProvider.This.GetTranslation(LangKeys.Attachment)}", $"[{t.FileName}]({UtilAttachment.GetUri(t.Id)})");

    return builder;
  }

  public static DiscordEmbedBuilder AddAttachment(this DiscordEmbedBuilder builder, IReadOnlyList<DiscordAttachment> discordAttachments) {
    if (discordAttachments == null || discordAttachments.Count == 0) return builder;
    for (var i = 0; i < discordAttachments.Count; i++)
      builder.AddField($"{LangProvider.This.GetTranslation(LangKeys.Attachment)} {i + 1}", discordAttachments[i].Url);
    return builder;
  }

  public static DiscordEmbedBuilder AddAttachment(this DiscordEmbedBuilder builder, DiscordAttachment discordAttachment) {
    builder.AddField($"{LangProvider.This.GetTranslation(LangKeys.Attachment)}", discordAttachment.Url);
    return builder;
  }

  public static DiscordEmbedBuilder WithCustomTimestamp(this DiscordEmbedBuilder builder) {
    return builder.WithTimestamp(UtilDate.GetNow());
  }

  public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder) {
    var sender = ServiceLocator.CreateSender();
    var guildInfo = sender.Send(new GetDiscordMainGuildQuery()).GetAwaiter().GetResult(); // TODO: Find a better way to handle this
    return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
  }

  public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder, GuildOption guildInfo) {
    return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
  }

  public static DiscordEmbedBuilder WithGuildInfoFooter(this DiscordEmbedBuilder builder, DiscordGuild guildInfo) {
    return builder.WithFooter(guildInfo.Name, guildInfo.IconUrl);
  }

  public static DiscordEmbedBuilder WithUserAsAuthor(this DiscordEmbedBuilder builder, DiscordUser user) {
    return builder.WithAuthor(user.GetUsername(), iconUrl: user.AvatarUrl);
  }

  public static DiscordEmbedBuilder WithUserAsAuthor(this DiscordEmbedBuilder builder, DiscordUserInfo user) {
    return builder.WithAuthor(user.Username, iconUrl: user.AvatarUrl);
  }
}