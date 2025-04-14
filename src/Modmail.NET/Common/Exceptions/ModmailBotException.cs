using DSharpPlus.Entities;
using Modmail.NET.Common.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class ModmailBotException : Exception
{
  public ModmailBotException(string titleMessage, string? contentMessage = null) {
    TitleMessage = titleMessage;
    ContentMessage = contentMessage;
  }

  public ModmailBotException(Lang titleMessage) {
    TitleMessage = titleMessage.Translate();
  }

  public ModmailBotException(Lang titleMessage, string[] param) {
    TitleMessage = titleMessage.Translate(param);
  }


  public ModmailBotException(Lang titleMessage, Lang contentMessage) {
    TitleMessage = titleMessage.Translate();
    ContentMessage = contentMessage.Translate();
  }


  public string TitleMessage { get; }
  public string? ContentMessage { get; }

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