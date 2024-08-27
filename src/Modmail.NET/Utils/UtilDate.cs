namespace Modmail.NET.Utils;

public static class UtilDate
{
  public static DateTime GetNow() {
    if (BotConfig.This.UseLocalTime) return DateTime.Now;

    return DateTime.UtcNow;
  }
}