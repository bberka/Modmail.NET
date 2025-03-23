using DSharpPlus.Entities;

namespace Modmail.NET.Static;

public static class Webhooks
{
  public static DiscordWebhookBuilder Error(string title, string message = "") {
    return new DiscordWebhookBuilder().AddEmbed(Embeds.Error(title, message));
  }

  public static DiscordWebhookBuilder Success(string title, string message = "") {
    return new DiscordWebhookBuilder().AddEmbed(Embeds.Success(title, message));
  }

  public static DiscordWebhookBuilder Info(string title, string message = "") {
    return new DiscordWebhookBuilder().AddEmbed(Embeds.Info(title, message));
  }

  public static DiscordWebhookBuilder Warning(string title, string message = "") {
    return new DiscordWebhookBuilder().AddEmbed(Embeds.Warning(title, message));
  }
}