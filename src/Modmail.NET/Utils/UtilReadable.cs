namespace Modmail.NET.Utils;

public static class UtilReadable
{
  public static string ConvertMinutesToReadableString(double? minutes) {
    return ConvertSecondsToReadableString(minutes * 60);
  }

  public static string ConvertSecondsToReadableString(double? seconds) {
    if (seconds is null or <= 0) return "0 secs";

    const double secondsPerMinute = 60;
    const double secondsPerHour = secondsPerMinute * 60;
    const double secondsPerDay = secondsPerHour * 24;
    const double secondsPerWeek = secondsPerDay * 7;
    const double secondsPerMonth = secondsPerDay * 30;
    const double secondsPerYear = secondsPerDay * 365;

    if (seconds >= secondsPerYear) return FormatNumber(seconds.Value / secondsPerYear) + " y";
    if (seconds >= secondsPerMonth) return FormatNumber(seconds.Value / secondsPerMonth) + " m";
    if (seconds >= secondsPerWeek) return FormatNumber(seconds.Value / secondsPerWeek) + " w";
    if (seconds >= secondsPerDay) return FormatNumber(seconds.Value / secondsPerDay) + " d";
    if (seconds >= secondsPerHour) return FormatNumber(seconds.Value / secondsPerHour) + " h";
    if (seconds >= secondsPerMinute) return FormatNumber(seconds.Value / secondsPerMinute) + " m";

    return FormatNumber(seconds.Value) + " s";
  }

  public static string ConvertNumberToReadableString(double? number) {
    if (number is null or 0) return "0";

    const double thousand = 1_000;
    const double million = 1_000_000;
    const double billion = 1_000_000_000;

    if (number < 0) return "-" + ConvertNumberToReadableString(-number);
    if (number >= billion) return FormatNumber(number.Value / billion) + " B";
    if (number >= million) return FormatNumber(number.Value / million) + " M";
    if (number >= thousand) return FormatNumber(number.Value / thousand) + " K";

    return FormatNumber(number.Value);
  }

  private static string FormatNumber(double number) {
    return number % 1 == 0
             ? number.ToString("F0")
             : number.ToString("F2");
  }
}