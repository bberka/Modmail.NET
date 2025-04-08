using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Extensions;

public static class ExceptionExtensions
{
  public static DiscordWebhookBuilder ToWebhookResponse(this ModmailBotException exception) {
    return ModmailWebhooks.Warning(exception.TitleMessage, exception.ContentMessage ?? "");
  }

  public static DiscordEmbedBuilder ToEmbedResponse(this ModmailBotException exception) {
    return ModmailEmbeds.Warning(exception.TitleMessage, exception.ContentMessage ?? "");
  }

  public static DiscordInteractionResponseBuilder ToInteractionResponse(this ModmailBotException exception) {
    return ModmailInteractions.Warning(exception.TitleMessage, exception.ContentMessage ?? "");
  }

  public static DiscordWebhookBuilder ToWebhookResponse(this Exception exception) {
    var config = ServiceLocator.GetBotConfig();

    if (config.Environment == EnvironmentType.Development) return ModmailWebhooks.Error(LangProvider.This.GetTranslation(LangKeys.AnExceptionOccurred), exception.Message);

    return ModmailWebhooks.Error(LangProvider.This.GetTranslation(LangKeys.AnExceptionOccurred));
  }

  public static DiscordEmbedBuilder ToEmbedResponse(this Exception exception) {
    var config = ServiceLocator.GetBotConfig();

    if (config.Environment == EnvironmentType.Development) return ModmailEmbeds.Error(LangProvider.This.GetTranslation(LangKeys.AnExceptionOccurred), exception.Message);

    return ModmailEmbeds.Error(LangProvider.This.GetTranslation(LangKeys.AnExceptionOccurred));
  }

  public static DiscordInteractionResponseBuilder ToInteractionResponse(this Exception exception) {
    var config = ServiceLocator.GetBotConfig();

    if (config.Environment == EnvironmentType.Development) return ModmailInteractions.Error(LangProvider.This.GetTranslation(LangKeys.AnExceptionOccurred), exception.Message);

    return ModmailInteractions.Error(LangProvider.This.GetTranslation(LangKeys.AnExceptionOccurred));
  }
}