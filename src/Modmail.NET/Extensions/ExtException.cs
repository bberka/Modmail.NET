using DSharpPlus.Entities;
using Modmail.NET.Abstract;

namespace Modmail.NET.Extensions;

public static class ExtException
{
  public static DiscordWebhookBuilder ToWebhookResponse(this BotExceptionBase exception) {
    return Webhooks.Warning(exception.TitleMessage, exception.ContentMessage ?? "");
  }

  public static DiscordEmbedBuilder ToEmbedResponse(this BotExceptionBase exception) {
    return Embeds.Warning(exception.TitleMessage, exception.ContentMessage ?? "");
  }

  public static DiscordInteractionResponseBuilder ToInteractionResponse(this BotExceptionBase exception) {
    return Interactions.Warning(exception.TitleMessage, exception.ContentMessage ?? "");
  }

  public static DiscordWebhookBuilder ToWebhookResponse(this Exception exception) {
    var config = ServiceLocator.GetBotConfig();

    if (config.Environment == EnvironmentType.Development) return Webhooks.Error(LangProvider.This.GetTranslation(LangKeys.AN_EXCEPTION_OCCURRED), exception.Message);

    return Webhooks.Error(LangProvider.This.GetTranslation(LangKeys.AN_EXCEPTION_OCCURRED));
  }

  public static DiscordEmbedBuilder ToEmbedResponse(this Exception exception) {
    var config = ServiceLocator.GetBotConfig();

    if (config.Environment == EnvironmentType.Development) return Embeds.Error(LangProvider.This.GetTranslation(LangKeys.AN_EXCEPTION_OCCURRED), exception.Message);

    return Embeds.Error(LangProvider.This.GetTranslation(LangKeys.AN_EXCEPTION_OCCURRED));
  }

  public static DiscordInteractionResponseBuilder ToInteractionResponse(this Exception exception) {
    var config = ServiceLocator.GetBotConfig();

    if (config.Environment == EnvironmentType.Development) return Interactions.Error(LangProvider.This.GetTranslation(LangKeys.AN_EXCEPTION_OCCURRED), exception.Message);

    return Interactions.Error(LangProvider.This.GetTranslation(LangKeys.AN_EXCEPTION_OCCURRED));
  }
}