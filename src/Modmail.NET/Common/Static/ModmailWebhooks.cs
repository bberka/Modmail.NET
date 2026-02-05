using DSharpPlus.Entities;

namespace Modmail.NET.Common.Static;

public static class ModmailWebhooks
{
    public static DiscordWebhookBuilder Error(string title, string message = "")
    {
        return new DiscordWebhookBuilder().AddEmbed(ModmailEmbeds.Error(title, message));
    }

    public static DiscordWebhookBuilder Success(string title, string message = "")
    {
        return new DiscordWebhookBuilder().AddEmbed(ModmailEmbeds.Success(title, message));
    }

    public static DiscordWebhookBuilder Info(string title, string message = "")
    {
        return new DiscordWebhookBuilder().AddEmbed(ModmailEmbeds.Info(title, message));
    }

    public static DiscordWebhookBuilder Warning(string title, string message = "")
    {
        return new DiscordWebhookBuilder().AddEmbed(ModmailEmbeds.Warning(title, message));
    }
}