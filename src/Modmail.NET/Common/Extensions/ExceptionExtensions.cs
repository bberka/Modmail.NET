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

		if (config.Environment == EnvironmentType.Development) return ModmailWebhooks.Error(Lang.AnExceptionOccurred.Translate(), exception.Message);

		return ModmailWebhooks.Error(Lang.AnExceptionOccurred.Translate());
	}

	public static DiscordEmbedBuilder ToEmbedResponse(this Exception exception) {
		var config = ServiceLocator.GetBotConfig();

		if (config.Environment == EnvironmentType.Development) return ModmailEmbeds.Error(Lang.AnExceptionOccurred.Translate(), exception.Message);

		return ModmailEmbeds.Error(Lang.AnExceptionOccurred.Translate());
	}

	public static DiscordInteractionResponseBuilder ToInteractionResponse(this Exception exception) {
		var config = ServiceLocator.GetBotConfig();

		if (config.Environment == EnvironmentType.Development) return ModmailInteractions.Error(Lang.AnExceptionOccurred.Translate(), exception.Message);

		return ModmailInteractions.Error(Lang.AnExceptionOccurred.Translate());
	}
}