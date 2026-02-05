using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Tag.Helpers;

public static class TagBotMessages
{
  public static DiscordMessageBuilder TagSent(Database.Entities.Tag message) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(message.Title)
                .WithDescription(message.Content)
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(ModmailColors.TagReceivedColor);

    var msg = new DiscordMessageBuilder();
    msg.AddEmbed(embed);
    return msg;
  }

  public static DiscordMessageBuilder TagReceivedToTicket(Database.Entities.Tag message, DiscordUser author = null, bool anonymous = false) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(message.Title)
                .WithDescription(message.Content)
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(ModmailColors.TagReceivedColor);

    if (author is not null)
      if (!anonymous)
        embed.WithUserAsAuthor(author);

    var msg = new DiscordMessageBuilder();
    msg.AddEmbed(embed);
    return msg;
  }
}