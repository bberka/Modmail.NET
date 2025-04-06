using DSharpPlus.Entities;

namespace Modmail.NET.Static;

public static class Const
{
  public const string CategoryName = "Modmail";
  public const string LogChannelName = "📄modmail-logs";
  public const string TicketNameTemplate = "ticket-{0}";
  public const string HighPriorityEmoji = "🔴";
  public const string NormalPriorityEmoji = "";
  public const string LowPriorityEmoji = "🟢";
  public const int TicketTimeoutMinAllowedHours = 12;
  public const int TicketTimeoutMaxAllowedHours = 24 * 7 * 4;
  public const int TicketDataDeleteWaitDaysMin = 1;
  public const int TicketDataDeleteWaitDaysMax = 365;
  public const int StatisticsCalculateDaysMin = 30;
  public const int StatisticsCalculateDaysMax = 365;
  public const int DefaultStatisticsCalculateDays = 90;
  public const string ThemeCookieName = "Modmail.NET.Theme";
  public const string AttachmentDownloadDirectory = "AttachmentDownloads";
  public const int HttpClientDownloadTimeoutSeconds = 90;
  public const string ProcessedReactionDiscordEmojiUnicode = "✅";
  public static readonly DiscordActivity DiscordActivity = new(LangProvider.This.GetTranslation(LangKeys.ModerationConcerns), DiscordActivityType.ListeningTo);
}