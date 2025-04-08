namespace Modmail.NET.Features.Ticket.Static;

public static class TicketConstants
{
  public const string TicketNameTemplate = "ticket-{0}";
  public const string HighPriorityEmoji = "🔴";
  public const string NormalPriorityEmoji = "";
  public const string LowPriorityEmoji = "🟢";

  public const int TicketTimeoutMinAllowedHours = 12;
  public const int TicketTimeoutMaxAllowedHours = 24 * 7 * 4;

  public const int TicketDataDeleteWaitDaysMin = 1;
  public const int TicketDataDeleteWaitDaysMax = 365;

  public const string ProcessedReactionDiscordEmojiUnicode = "✅";
}