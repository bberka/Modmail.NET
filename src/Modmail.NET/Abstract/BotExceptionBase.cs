using DSharpPlus.Entities;

namespace Modmail.NET.Abstract;

public abstract class BotExceptionBase : Exception
{
  protected BotExceptionBase(string titleMessage, string contentMessage = null) {
    TitleMessage = titleMessage;
    ContentMessage = contentMessage;
  }

  public string TitleMessage { get; }
  public string ContentMessage { get; }

  public DiscordWebhookBuilder GetWebhookResponse() {
    return Webhooks.Warning(TitleMessage, ContentMessage ?? "");
  }

  public DiscordEmbedBuilder GetEmbedResponse() {
    return Embeds.Warning(TitleMessage, ContentMessage ?? "");
  }

  public DiscordInteractionResponseBuilder GetInteractionResponse() {
    return Interactions.Warning(TitleMessage, ContentMessage ?? "");
  }
}