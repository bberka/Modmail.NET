using DSharpPlus.Entities;

namespace Modmail.NET.Static;

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
}