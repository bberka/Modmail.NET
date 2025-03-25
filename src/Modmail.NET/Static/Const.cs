using DSharpPlus.Entities;

namespace Modmail.NET.Static;

public static class Const
{
  public const string DefaultPrefix = "!";
  public const string ApplicationName = "Modmail.NET";
  public const string BotName = "Modmail";
  public const string CategoryName = "Modmail";
  public const string LogChannelName = "📄modmail-logs";


  public const string TicketNameTemplate = "ticket-{0}";
  public const string HighPriorityEmoji = "🔴";
  public const string NormalPriorityEmoji = "";
  public const string LowPriorityEmoji = "🟢";
  public const int DbTimeout = 10;
  public const int DefaultTicketTimeoutHours = 72;
  public const int TicketTimeoutMinAllowedHours = 12;
  public const int TicketTimeoutMaxAllowedHours = 168 * 2; // 2 weeks
  public const string ThemeCookieName = "Modmail.NET.Theme";
  public static readonly DiscordActivity DiscordActivity = new(LangProvider.This.GetTranslation(LangKeys.ModerationConcerns), DiscordActivityType.ListeningTo);
}