using DSharpPlus.Entities;

namespace Modmail.NET.Common.Static;

public static class ModmailEmbeds
{
    public static DiscordEmbedBuilder Error(string title, string message = "")
    {
        return new DiscordEmbedBuilder().WithTitle(title)
            .WithDescription(message)
            .WithColor(ModmailColors.ErrorColor);
    }

    public static DiscordEmbedBuilder Success(string title, string message = "")
    {
        return new DiscordEmbedBuilder().WithTitle(title)
            .WithDescription(message)
            .WithColor(ModmailColors.SuccessColor);
    }

    public static DiscordEmbedBuilder Info(string title, string message = "")
    {
        return new DiscordEmbedBuilder().WithTitle(title)
            .WithDescription(message)
            .WithColor(ModmailColors.InfoColor);
    }

    public static DiscordEmbedBuilder Warning(string title, string message = "")
    {
        return new DiscordEmbedBuilder().WithTitle(title)
            .WithDescription(message)
            .WithColor(ModmailColors.WarningColor);
    }
}