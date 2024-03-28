using DSharpPlus.Entities;

namespace Modmail.NET.Common;

public static class Embeds
{
  public static DiscordEmbedBuilder Error(string title, string message = "") {
    return new DiscordEmbedBuilder()
           .WithTitle(title)
           .WithDescription(message)
           .WithColor(Colors.ErrorColor);
  }

  public static DiscordEmbedBuilder Success(string title, string message = "") {
    return new DiscordEmbedBuilder()
           .WithTitle(title)
           .WithDescription(message)
           .WithColor(Colors.SuccessColor);
  }

  public static DiscordEmbedBuilder Info(string title, string message = "") {
    return new DiscordEmbedBuilder()
           .WithTitle(title)
           .WithDescription(message)
           .WithColor(Colors.InfoColor);
  }

  public static DiscordEmbedBuilder Warning(string title, string message = "") {
    return new DiscordEmbedBuilder()
           .WithTitle(title)
           .WithDescription(message)
           .WithColor(Colors.WarningColor);
  }


  // public static DiscordEmbed Base(string title, string text = "", DiscordColor? color = null) {
  //   color ??= DiscordColor.White;
  //   var embed = new DiscordEmbedBuilder()
  //               .WithTitle(title)
  //               .WithColor(color.Value);
  //   if (!string.IsNullOrEmpty(text))
  //     embed.WithDescription(text);
  //   return embed;
  // }

  // public static class ToMail
  // {
  //
  //  
  //
  //   // public static DiscordEmbed MessageSent(DiscordUser author, DiscordMessage message, bool ticketAnonymous) {
  //   //   var embed = new DiscordEmbedBuilder()
  //   //               .WithDescription(message.Content)
  //   //               .WithCustomTimestamp()
  //   //               .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
  //   //               .WithColor(Colors.MessageSentColor);
  //   //
  //   //   for (var i = 0; i < message.Attachments.Count; i++)
  //   //     embed.AddField($"{Texts.ATTACHMENT} {i + 1}", message.Attachments[i].Url);
  //   //
  //   //   if (ticketAnonymous) embed.WithFooter(Texts.ANONYMOUS_MESSAGE);
  //   //
  //   //   return embed;
  //   // }
  //   //
  //
  //
  // }
}