namespace Modmail.NET.Common;

public static class CommonHelper
{
  public static string GetStringOrNaN(this string value) {
    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) return "NaN";

    return value;
  }
}