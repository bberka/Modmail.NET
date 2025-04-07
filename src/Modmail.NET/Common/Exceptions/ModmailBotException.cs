using DSharpPlus.Entities;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Common.Exceptions;

public abstract class ModmailBotException : Exception
{
  protected ModmailBotException(string titleMessage, string contentMessage = null) {
    TitleMessage = titleMessage;
    ContentMessage = contentMessage;
  }

  public string TitleMessage { get; }
  public string ContentMessage { get; }

  public DiscordWebhookBuilder GetWebhookResponse() {
    return ModmailWebhooks.Warning(TitleMessage, ContentMessage ?? "");
  }

  public DiscordEmbedBuilder GetEmbedResponse() {
    return ModmailEmbeds.Warning(TitleMessage, ContentMessage ?? "");
  }

  public DiscordInteractionResponseBuilder GetInteractionResponse() {
    return ModmailInteractions.Warning(TitleMessage, ContentMessage ?? "");
  }
}