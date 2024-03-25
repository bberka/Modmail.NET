using DSharpPlus.Entities;

namespace Modmail.NET.Common;

public static class Interactions
{
  public static DiscordInteractionResponseBuilder Error(string title, string message = "") {
    return new DiscordInteractionResponseBuilder().AddEmbed(Embeds.Error(title, message));
  }

  public static DiscordInteractionResponseBuilder Success(string title, string message = "") {
    return new DiscordInteractionResponseBuilder().AddEmbed(Embeds.Success(title, message));
  }

  public static DiscordInteractionResponseBuilder Info(string title, string message = "") {
    return new DiscordInteractionResponseBuilder().AddEmbed(Embeds.Info(title, message));
  }

  public static DiscordInteractionResponseBuilder Warning(string title, string message = "") {
    return new DiscordInteractionResponseBuilder().AddEmbed(Embeds.Warning(title, message));
  }
}