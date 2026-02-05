using DSharpPlus.Entities;
using Modmail.NET.Common.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class ModmailBotException : Exception
{
  [Obsolete("String message constructor is obsolete and will be removed, use constructors that takes Language keys")]
  protected ModmailBotException(string titleMessage, string contentMessage = null) {
    TitleMessage = titleMessage;
    ContentMessage = contentMessage;
  }

  public ModmailBotException(LangKeys titleMessage) {
    TitleMessage = titleMessage.GetTranslation();
  }

  public ModmailBotException(LangKeys titleMessage, LangKeys contentMessage) {
    TitleMessage = titleMessage.GetTranslation();
    ContentMessage = contentMessage.GetTranslation();
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