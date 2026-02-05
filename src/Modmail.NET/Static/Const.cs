using DSharpPlus.Entities;

namespace Modmail.NET.Static;

public static class Const
{
    public const string DEFAULT_PREFIX = "!";
    public const string APPLICATION_NAME = "Modmail.NET";
    public const string BOT_NAME = "Modmail";
    public const string CATEGORY_NAME = "Modmail";
    public const string LOG_CHANNEL_NAME = "📄modmail-logs";


    public const string TICKET_NAME_TEMPLATE = "ticket-{0}";
    public const string HIGH_PRIORITY_EMOJI = "🔴";
    public const string NORMAL_PRIORITY_EMOJI = "";
    public const string LOW_PRIORITY_EMOJI = "🟢";
    public const int DB_TIMEOUT = 10;
    public const int DEFAULT_TICKET_TIMEOUT_HOURS = 72;
    public const int TICKET_TIMEOUT_MIN_ALLOWED_HOURS = 12;
    public const int TICKET_TIMEOUT_MAX_ALLOWED_HOURS = 168 * 2; // 2 weeks

    public static readonly DiscordActivity DISCORD_ACTIVITY =
        new(LangData.This.GetTranslation(LangKeys.MODERATION_CONCERNS), ActivityType.ListeningTo);
}