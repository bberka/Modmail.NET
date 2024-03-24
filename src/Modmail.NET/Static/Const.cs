using DSharpPlus.Entities;

namespace Modmail.NET.Static;

public static class Const
{
  public const string DEFAULT_PREFIX = "!";
  public const string APPLICATION_NAME = "Modmail.NET";
  public const string BOT_NAME = "Modmail";
  public const string CATEGORY_NAME = "Modmail";
  public const string LOG_CHANNEL_NAME = "📄modmail-logs";


  public const string TICKET_NAME_TEMPLATE = NORMAL_PRIORITY_EMOJI + "ticket-{0}";
  public const string HIGH_PRIORITY_EMOJI = "🔴";
  public const string NORMAL_PRIORITY_EMOJI = "⚪";
  public const string LOW_PRIORITY_EMOJI = "🟢";
  public const int DB_TIMEOUT = 10;
  public static readonly DiscordActivity DISCORD_ACTIVITY = new(Texts.MODERATION_CONCERNS, ActivityType.ListeningTo);
}