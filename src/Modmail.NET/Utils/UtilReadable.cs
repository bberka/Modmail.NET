namespace Modmail.NET.Utils;

public static class UtilReadable
{
  public static string ConvertMinutesToReadableString(double? minutes) {
    if (minutes is null) {
      return "0.00";
    }
    if (minutes > 60) {
      return (minutes.Value / 60).ToString("F") + " hours";
    }
    
    if (minutes > 60 * 24) {
      return (minutes.Value / 60 * 24).ToString("F") + " days";
    }
    
    return minutes.Value.ToString("F") + " mins";
  }
}