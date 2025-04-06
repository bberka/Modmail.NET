using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Features.Bot;
using Modmail.NET.Utils;

namespace Modmail.NET.Extensions;

public static class ExtEmbed
{
  public static DiscordMessageBuilder AddAttachments(this DiscordMessageBuilder builder, TicketMessageAttachment[] attachments) {
    if (attachments == null || attachments.Length == 0) return builder;

    foreach (var attachment in attachments) {
      var path = UtilAttachment.GetLocalPath(attachment);
      builder.AddFile(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), AddFileOptions.None);
    }

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