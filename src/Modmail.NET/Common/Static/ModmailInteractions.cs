using DSharpPlus.Entities;

namespace Modmail.NET.Common.Static;

public static class ModmailInteractions
{
    public static DiscordInteractionResponseBuilder Error(string title, string message = "")
    {
        return new DiscordInteractionResponseBuilder().AddEmbed(ModmailEmbeds.Error(title, message));
    }

    public static DiscordInteractionResponseBuilder Success(string title, string message = "")
    {
        return new DiscordInteractionResponseBuilder().AddEmbed(ModmailEmbeds.Success(title, message));
    }

    public static DiscordInteractionResponseBuilder Info(string title, string message = "")
    {
        return new DiscordInteractionResponseBuilder().AddEmbed(ModmailEmbeds.Info(title, message));
    }

    public static DiscordInteractionResponseBuilder Warning(string title, string message = "")
    {
        return new DiscordInteractionResponseBuilder().AddEmbed(ModmailEmbeds.Warning(title, message));
    }
}